using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DiscUtils.Iso9660;
using DiscUtils;
using MomenTFS.UI;
using MomenTFS.MAP;
using System.Collections.ObjectModel;
using System;
using MomenTFS.Objects;
using MomenTFS.MAP.Collision;

namespace MomenTFS.Forms
{
    public partial class MainForm : Form
    {
        private RoomReader roomReader;
        private RoomData roomData;
        private ObservableCollection<TFSFile> files;
        private Stream isoFileStream;
        private CDReader cdReader;
        private ListBox fileList;
        private TabPage timDataPage;
        private MAPObjectsPage mapObjectsPage;

        public MainForm() {
            Title = "MomenTFS";
            ClientSize = new Size(600, 480);
            roomReader = new RoomReader();

            files = new ObservableCollection<TFSFile>();
            files.CollectionChanged += (sender, e) => {
                fileList.DataStore = files;
                fileList.UpdateBindings();
            };

            var imageView = new ImageView();
            var scrollable = new Scrollable {
                Content = imageView,
                ExpandContentWidth = false,
                ExpandContentHeight = false
            };

            var paletteDropdown = new DropDown {
                Visible = false,
            };
            var paletteDropdownLabel = new Label {
                Visible = false,
                Text = "Palette",
            };

            paletteDropdown.SelectedIndexChanged
                += (sender, e) => RenderTFS(paletteDropdown, imageView);

            var tabControl = new TabControl();
            var previewLayout = new DynamicLayout();
            var toolbar = new DynamicLayout();

            var toolbarImageInfoLabel = new Label();
            toolbar.AddRow(
                toolbarImageInfoLabel,
                null,
                paletteDropdownLabel,
                paletteDropdown);

            previewLayout.BeginVertical();
            previewLayout.AddRow(toolbar);
            previewLayout.AddRow(scrollable);
            previewLayout.EndVertical();

            var previewPage = new TabPage { Text = "Preview" };
            previewPage.Content = previewLayout;
            tabControl.Pages.Add(previewPage);

            var mapDetailsText = new TextArea { ReadOnly = true };
            var mapDetailsPage = new TabPage { Text = "Details" };
            mapDetailsPage.Content = mapDetailsText;
            tabControl.Pages.Add(mapDetailsPage);

            timDataPage = new TabPage { Text = "TIM Data" };
            tabControl.Pages.Add(timDataPage);

            var collisionImageView = new ImageView();
            var collisionMapPage = new TabPage { Text = "Collision Map" };
            collisionMapPage.Content = new Scrollable {
                Content = collisionImageView,
                ExpandContentWidth = false,
                ExpandContentHeight = false
            };
            tabControl.Pages.Add(collisionMapPage);

            mapObjectsPage = new MAPObjectsPage(tabControl);

            fileList = new ListBox() {
                Width = 200
            };

            var layout = new DynamicLayout();
            layout.BeginVertical();
            layout.BeginHorizontal();
            layout.BeginVertical();
            layout.AddRow(new Label { Text = "Files" });
            layout.AddRow(fileList);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(tabControl);
            layout.EndVertical();
            layout.EndHorizontal();
            layout.EndVertical();

            Content = layout;

            var aboutDialog = new AboutDialog() {
                Developers = new string[1] { "Samuel Kinnett" },
                ProgramName = "MomenTFS",
                Version = "",
                ProgramDescription
                = "A TFS viewer based on and inspired by TFSViewer by Lab 313.\n\n" +
                "Huge thanks to Romsstar and SydMontague for their work in reverse engineering " +
                "the TFS and MAP file formats!",
                License = License.LicenseString
            };

            var quitCommand = new Command {
                MenuText = "Quit",
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };
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
                Enabled = false
            };

            loadISO.Executed += (sender, e) => OpenISO(this, loadISO, unloadISO);
            unloadISO.Executed += (sender, e) => CloseISO(loadISO, unloadISO);

            fileList.SelectedIndexChanged += (sender, e)
                => OpenSelectedTFS(
                    saveImage,
                    paletteDropdown,
                    paletteDropdownLabel,
                    toolbarImageInfoLabel,
                    imageView,
                    collisionImageView,
                    mapDetailsText);
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
                    TFSFile newTFSFile = new TFSFile(tfsFilename, filename);
                    var directoryPath = Path.GetDirectoryName(tfsFilename);
                    var newMAPFile = Path.Combine(
                        directoryPath, $"{Path.GetFileNameWithoutExtension(tfsFilename)}.MAP;1");

                    if (File.Exists(newMAPFile)) {
                        newTFSFile.MAPFilename = newMAPFile;
                    }

                    files.Add(newTFSFile);
                }
            }
        }

        private void CloseISO(Command loadISO, Command closeISO) {
            cdReader.Dispose();
            isoFileStream.Dispose();

            var filesToRemove = files.Where((file) => file.DiscFile != null);

            foreach (var file in filesToRemove) {
                files.Remove(file);
            }

            closeISO.Enabled = false;
            loadISO.Enabled = true;
        }

        private List<string> ScanDiscDirectory(DiscDirectoryInfo directory) {
            List<string> tfsFiles = new List<string>();

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
                Label toolbarImageInfoLabel,
                ImageView imageView,
                ImageView collisionMapImageView,
                TextArea mapDetailsText) {

            if (fileList.SelectedKey == null) {
                imageView.Image = null;
                return;
            }

            saveImage.Enabled = false;
            paletteDropdown.Visible = false;
            paletteDropdown.Items.Clear();
            paletteDropdownLabel.Visible = false;
            saveImage.Enabled = false;
            toolbarImageInfoLabel.Text = "";
            mapDetailsText.Text = "";
            timDataPage.Content = null;

            TFSFile selectedItem = files[fileList.SelectedIndex];
            if (selectedItem.DiscFile == null) {
                roomData = selectedItem.MAPFilename == null
                    ? roomReader.ReadRoomDataTFSOnly(selectedItem.Filename)
                    : roomReader.ReadRoomData(selectedItem.Filename, selectedItem.MAPFilename);
            } else {
                using (Stream fileStream
                        = cdReader.OpenFile(selectedItem.Filename, FileMode.Open)) {
                    if (selectedItem.MAPFilename == null) {
                        roomData = roomReader.ReadRoomDataTFSOnly(fileStream);
                    } else {
                        using (Stream mapFileStream
                                = cdReader.OpenFile(selectedItem.MAPFilename, FileMode.Open)) {
                            roomData = roomReader.ReadRoomData(fileStream, mapFileStream);
                        }
                    }
                }
            }

            if (roomData.MAPData != null) {
                mapDetailsText.Text = getDetailsText(roomData.MAPData);
                var timDataTabControl = new TabControl();
                for (var i = 0; i < roomData.MAPData.TIMImages.Count; ++i) {
                    var timImageView = new ImageView();
                    var timBitmap = roomData.MAPData.TIMImages[i].GetBitmapAsFlatArray(roomData.TFSData.ColourLookupTable);
                    timImageView.Image
                        = getBitmap(roomData.MAPData.TIMImages[i].ImageSize, timBitmap);

                    var tabPage = new TabPage { Text = $"Image {i + 1}" };
                    tabPage.Content = timImageView;
                    timDataTabControl.Pages.Add(tabPage);
                }

                timDataPage.Content = timDataTabControl;

                List<Color> bitmapData = new List<Color>();
                var collisionMapWidth = roomData.MAPData.Collision.CollisionMap.GetLength(0);
                var collisionMapHeight = roomData.MAPData.Collision.CollisionMap.GetLength(1);
                for (int y = 0; y < collisionMapHeight; ++y) {
                    for (int x = 0; x < collisionMapWidth; ++x) {
                        CollisionType collisionType
                            = roomData.MAPData.Collision.CollisionMap[x, y];

                        switch (collisionType) {
                            case CollisionType.SOLID:
                                bitmapData.Add(Color.FromArgb(29, 31, 33));
                                break;
                            case CollisionType.EMPTY:
                                bitmapData.Add(Color.FromArgb(197, 200, 198));
                                break;
                            case CollisionType.EXIT_1:
                            case CollisionType.EXIT_2:
                            case CollisionType.EXIT_3:
                            case CollisionType.EXIT_4:
                            case CollisionType.EXIT_5:
                            case CollisionType.EXIT_6:
                            case CollisionType.EXIT_7:
                            case CollisionType.EXIT_8:
                            case CollisionType.EXIT_9:
                            case CollisionType.EXIT_10:
                                bitmapData.Add(Color.FromArgb(181, 189, 104));
                                break;
                            case CollisionType.TOILET:
                                bitmapData.Add(Color.FromArgb(129, 162, 190));
                                break;
                            default:
                                bitmapData.Add(Color.FromArgb(133, 103, 143));
                                break;
                        }
                    }
                }

                collisionMapImageView.Image
                    = new Bitmap(
                        collisionMapWidth,
                        collisionMapHeight,
                        PixelFormat.Format32bppRgba,
                        bitmapData);

                mapObjectsPage.Update(roomData);
            }

            toolbarImageInfoLabel.Text
                = $"{roomData.TFSData.ImageSize.X} x {roomData.TFSData.ImageSize.Y} pixels";

            var tfsBitmap = roomData.TFSData.GetBitmapAsFlatArray(0);
            imageView.Image = getBitmap(roomData.TFSData.ImageSize, tfsBitmap);

            if (roomData.TFSData.PaletteCount > 1) {
                List<string> options = Enumerable
                    .Range(0, roomData.TFSData.PaletteCount)
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
                var systemBitmap = roomData.TFSData.GetBitmapAsFlatArray(
                    int.Parse(paletteDropdown.SelectedKey));
                bitmapToSave = getBitmap(roomData.TFSData.ImageSize, systemBitmap);

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
                var systemBitmap = roomData.TFSData.GetBitmapAsFlatArray(
                    int.Parse(paletteDropdown.SelectedKey));
                imageView.Image = getBitmap(roomData.TFSData.ImageSize, systemBitmap);
            }
        }

        private string getDetailsText(MAPData mapData) {
            var detailsText = $"Camera Origin: {mapData.Settings.CameraOrigin}\n" +
                $"Camera Translation: {mapData.Settings.CameraTranslation}\n";

            for (int i = 0; i < 3; ++i) {
                detailsText += $"Light {i}:\n\t{mapData.Settings.Lights[i]}\n";
            }

            detailsText += $"Zoom: {mapData.Settings.Zoom}\n" +
                $"Sprite Scale: {mapData.Settings.SpriteScale}\n" +
                $"Area Like Types: [{string.Join(", ", mapData.Settings.AreaLikeTypes)}]\n" +
                $"Area Dislike Types: [{string.Join(", ", mapData.Settings.AreaDislikeTypes)}]\n" +
                $"Map Tile Width: {mapData.Settings.MapTileWidth}\n" +
                $"Map Tile Height: {mapData.Settings.MapTileHeight}\n" +
                $"Map Tiles:\n";

            for (int y = 0; y < mapData.Settings.MapTileHeight; ++y) {
                detailsText += "\t";
                for (int x = 0; x < mapData.Settings.MapTileWidth; ++x) {
                    detailsText += $"[{mapData.Settings.MapTiles[x, y]}]";
                }
                detailsText += "\n";
            }

            return detailsText;
        }

        private Bitmap getBitmap(
                IVector2 dimensions,
                System.Drawing.Color[] bitmapData,
                bool withTransparency = false) {
            List<Color> convertedBitmapData = Array
                .ConvertAll(bitmapData, color
                    => convertSystemColorToEtoColor(color, withTransparency))
                .ToList();

            return new Bitmap(
                dimensions.X,
                dimensions.Y,
                PixelFormat.Format32bppRgba,
                convertedBitmapData);
        }

        private Color convertSystemColorToEtoColor(
                System.Drawing.Color color, bool withTransparency) {
            return withTransparency
                ? new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f)
                : new Color(color.R / 255f, color.G / 255f, color.B / 255f);
        }
    }
}
