using System;
using Eto.Forms;
using Eto.Drawing;
using MomenTFS.Reader;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DiscUtils.Iso9660;
using DiscUtils;
using MomenTFS.UI;
using MomenTFS.Forms.Extensions;
using MomenTFS.MAP;

namespace MomenTFS.Forms
{
    public partial class MainForm : Form
    {
        private TFSReader tfsReader;
        private MAPReader mapReader;
        private List<TFSFile> files;
        private Stream isoFileStream;
        private CDReader cdReader;
        private ListBox fileList;

        public MainForm() {
            Title = "MomenTFS";
            ClientSize = new Size(600, 480);
            tfsReader = new TFSReader();
            mapReader = new MAPReader();
            files = new List<TFSFile>();

            var paletteDropdown = new DropDown {
                Visible = false,
            };
            var paletteDropdownLabel = new Label {
                Visible = false,
                Text = "Palette"
            };

            var imageView = new ImageView();
            var scrollable = new Scrollable {
                Content = imageView,
                ExpandContentWidth = false,
                ExpandContentHeight = false
            };

            fileList = new ListBox() {
                Width = 200
            };

            var aboutDialog = new AboutDialog() {
                Developers = new string[1] { "Samuel Kinnett" },
                ProgramName = "MomenTFS",
                Version = "",
                ProgramDescription = "A TFS viewer based on and inspired by TFSViewer by Lab 313",
                License = License.LicenseString
            };

            scrollable.Content = imageView;
            paletteDropdown.SelectedIndexChanged
                += (sender, e) => RenderTFS(paletteDropdown, imageView);


            var toolbar = new DynamicLayout();
            toolbar.AddRow(
                new Label { Text = "Preview" },
                null,
                paletteDropdownLabel,
                paletteDropdown);

            var layout = new DynamicLayout();
            layout.BeginVertical();
            layout.BeginHorizontal();
            layout.BeginVertical();
            layout.AddRow(new Label { Text = "Files" });
            layout.AddRow(fileList);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(toolbar);
            layout.AddRow(scrollable);
            layout.EndVertical();
            layout.EndHorizontal();
            layout.EndVertical();

            Content = layout;

            var quitCommand = new Command {
                MenuText = "Quit",
                Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => aboutDialog.ShowDialog(this);

            var saveImage = new Command { MenuText = "Save Image", ToolBarText = "Save Image" };
            saveImage.Enabled = false;
            saveImage.Executed += (sender, e) => SaveImage(this, paletteDropdown);

            var loadTFS = new Command { MenuText = "Open TFS", ToolBarText = "Load TFS" };
            loadTFS.Executed += (sender, e) => OpenTFS(this);

            var loadISO = new Command { MenuText = "Open ISO", ToolBarText = "Open ISO" };
            var unloadISO = new Command {
                MenuText = "Unload ISO",
                ToolBarText = "Unload ISO",
                Enabled = false };

            loadISO.Executed += (sender, e) => OpenISO(this, loadISO, unloadISO);
            unloadISO.Executed += (sender, e) => CloseISO(loadISO, unloadISO);

            fileList.SelectedIndexChanged += (sender, e) => OpenSelectedTFS(
                saveImage, paletteDropdown, paletteDropdownLabel, imageView);
            fileList.KeyDown += (sender, e) => { 
                if (e.Key == Keys.Delete || e.Key == Keys.Backspace) { RemoveTFS(); } 
            };

            fileList.DataStore = files;
            fileList.ItemTextBinding = Binding.Property(
                (TFSFile tfsFile) => Path.GetFileNameWithoutExtension(tfsFile.Filename));

            // create menu
            Menu = new MenuBar {
                Items =
                {
					// File submenu
					new ButtonMenuItem {
                        Text = "&File",
                        Items = { loadTFS, loadISO, unloadISO, saveImage }
                    }
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
                ApplicationItems =
                {
					// application (OS X) or file menu (others)
					// new ButtonMenuItem { Text = "&Preferences..." },
                },
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };

            // create toolbar			
            // ToolBar = new ToolBar { Items = { clickMe, loadTFS } };
        }

        private void OpenTFS(Control control) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            FileFilter tfsFilter = new FileFilter("TFS", ".tfs", ".TFS");

            openFileDialog.MultiSelect = true;
            openFileDialog.Filters.Add(tfsFilter);
            openFileDialog.ShowDialog(control);

            foreach (var filename in openFileDialog.Filenames) {
                if (!string.IsNullOrEmpty(filename)
                        && !files.Any(i => i.Filename == filename)) {
                    var newFile = new TFSFile(filename);
                    var directoryPath = Path.GetDirectoryName(filename);
                    var newMAPFile = Path.Combine(
                        directoryPath, $"{Path.GetFileNameWithoutExtension(filename)}.MAP");

                    if (File.Exists(newMAPFile)) {
                        newFile.MAPFilename = newMAPFile;
                    }

                    files.Add(newFile);
                }
            }

            fileList.DataStore = files;
            fileList.UpdateBindings();
        }

        private void OpenISO(Control control, Command loadISO, Command closeISO) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            FileFilter isoFilter = new FileFilter("ISO", ".iso", ".ISO");

            openFileDialog.MultiSelect = false;
            openFileDialog.Filters.Add(isoFilter);
            openFileDialog.ShowDialog(control);

            string filename = openFileDialog.FileName;

            if (!string.IsNullOrEmpty(filename)) {
                isoFileStream = File.Open(filename, FileMode.Open);
                cdReader = new CDReader(isoFileStream, true);
                loadISO.Enabled = false;
                closeISO.Enabled = true;
                var tfsFiles = ScanDiscDirectory(cdReader.Root);
                foreach (string tfsFilename in tfsFiles) {
                    files.Add(new TFSFile(tfsFilename, filename));
                }
            }

            fileList.DataStore = files;
            fileList.UpdateBindings();
        }

        private void CloseISO(Command loadISO, Command closeISO) {
            cdReader.Dispose();
            isoFileStream.Dispose();

            files.RemoveAll((file) => file.DiscFile != null);
            fileList.DataStore = files;
            fileList.UpdateBindings();

            closeISO.Enabled = false;
            loadISO.Enabled = true;
        }

        private List<String> ScanDiscDirectory(DiscDirectoryInfo directory) {
            List<String> tfsFiles = new List<string>();

            var files = directory.GetFiles();
            var subDirectories = directory.GetDirectories();

            foreach (var file in files) {
                if (file.Extension.ToLower() == "tfs;1") {
                    tfsFiles.Add(file.FullName);
                }
            }

            foreach (var subDirectory in subDirectories) {
                tfsFiles.AddRange(ScanDiscDirectory(subDirectory));
            }

            return tfsFiles;
        }

        private void OpenSelectedTFS(
                Command saveImage,
                DropDown paletteDropdown,
                Label paletteDropdownLabel,
                ImageView imageView) {

            if (fileList.SelectedKey == null) {
                imageView.Image = null;
                return;
            }

            saveImage.Enabled = false;
            paletteDropdown.Visible = false;
            paletteDropdown.Items.Clear();
            paletteDropdownLabel.Visible = false;
            saveImage.Enabled = false;

            TFSFile selectedItem = files[fileList.SelectedIndex];
            if (selectedItem.DiscFile == null) {
                tfsReader.Read(selectedItem.Filename);
            } else {
                using (Stream fileStream
                        = cdReader.OpenFile(selectedItem.Filename, FileMode.Open)) {
                    tfsReader.Read(fileStream);
                }
            }

            if (selectedItem.MAPFilename != null) {
                if (selectedItem.DiscFile == null) {
                    mapReader.Read(selectedItem.Filename);
                } else {
                    using (Stream fileStream
                            = cdReader.OpenFile(selectedItem.Filename, FileMode.Open)) {
                        mapReader.Read(fileStream);
                    }
                }
            }

            using (var systemBitmap = tfsReader.RenderImage(0)) {
                imageView.Image = systemBitmap.ToEtoBitmap();
            }

            if (tfsReader.PaletteCount > 1) {
                List<string> options = Enumerable
                    .Range(0, tfsReader.PaletteCount)
                    .Select(i => i.ToString())
                    .ToList();

                foreach (var option in options) {
                    paletteDropdown.Items.Add(option);
                }

                paletteDropdown.SelectedKey = "0";
                paletteDropdown.Visible = true;
                paletteDropdownLabel.Visible = true;
            }

            saveImage.Enabled = true;
        }

        private void RemoveTFS() {
            if (fileList.SelectedIndex > -1) {
                int currentIndex = fileList.SelectedIndex;
                files.RemoveAt(currentIndex);
                fileList.DataStore = files;
                if (currentIndex < files.Count) {
                    fileList.SelectedIndex = currentIndex;
                } else if (currentIndex > 0) {
                    fileList.SelectedIndex = currentIndex - 1;
                }
            }
        }

        private void SaveImage(Control control, DropDown paletteDropdown) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filters.Add(new FileFilter("BMP", ".bmp"));
            saveFileDialog.Filters.Add(new FileFilter("JPEG", ".jpeg"));
            saveFileDialog.Filters.Add(new FileFilter("PNG", ".png"));
            saveFileDialog.CurrentFilterIndex = 0;
            saveFileDialog.ShowDialog(control);

            if (!string.IsNullOrEmpty(saveFileDialog.FileName)) {
                Bitmap bitmapToSave;
                using (var systemBitmap
                        = tfsReader.RenderImage(int.Parse(paletteDropdown.SelectedKey))) {
                    bitmapToSave = systemBitmap.ToEtoBitmap();
                }
                switch (saveFileDialog.CurrentFilter.Name) {
                    case "BMP":
                        bitmapToSave.Save(saveFileDialog.FileName, ImageFormat.Bitmap);
                        break;
                    case "JPEG":
                        bitmapToSave.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case "PNG":
                        bitmapToSave.Save(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                }
            }
        }

        private void RenderTFS(DropDown paletteDropdown, ImageView imageView) {
            if (paletteDropdown.Visible) {
                using (var systemBitmap
                        = tfsReader.RenderImage(int.Parse(paletteDropdown.SelectedKey))) {
                    imageView.Image = systemBitmap.ToEtoBitmap();
                }
            }
        }
    }
}
