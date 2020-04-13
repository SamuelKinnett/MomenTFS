using System;
using Eto.Forms;
using Eto.Drawing;
using MomenTFS.Reader;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace MomenTFS
{
    public partial class MainForm : Form
    {
        TFSReader tfsReader;

        public MainForm() {
            Title = "MomenTFS";
            ClientSize = new Size(600, 480);
            tfsReader = new TFSReader();

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

            var aboutDialog = new AboutDialog() {
                Developers = new string[1] { "Samuel Kinnett" },
                Version = "",
                ProgramDescription = "A TFS viewer based on and inspired by TFSViewer by Lab 313",
            };

            scrollable.Content = imageView;
            paletteDropdown.SelectedValueChanged += (sender, e) => RenderTFS(paletteDropdown, imageView);

            var layout = new DynamicLayout();
            layout.BeginVertical();
            layout.AddRow(new Label { Text = "Preview" }, null, paletteDropdownLabel, paletteDropdown);
            layout.EndVertical();

            layout.BeginVertical();
            layout.AddRow(scrollable);
            layout.EndVertical();

            Content = layout;

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => aboutDialog.ShowDialog(this);

            var saveImage = new Command { MenuText = "Save Image", ToolBarText = "Save Image" };
            saveImage.Enabled = false;
            saveImage.Executed += (sender, e) => SaveImage(this, paletteDropdown);

            var loadTFS = new Command { MenuText = "Load TFS", ToolBarText = "Load TFS" };
            loadTFS.Executed += (sender, e) => OpenTFS(this, imageView, paletteDropdown, paletteDropdownLabel, saveImage);

            // create menu
            Menu = new MenuBar {
                Items =
                {
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = { loadTFS, saveImage } }
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
                ApplicationItems =
                {
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
                },
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };

            // create toolbar			
            // ToolBar = new ToolBar { Items = { clickMe, loadTFS } };
        }

        private void OpenTFS(Control control, ImageView imageView, DropDown paletteDropdown, Label paletteDropdownLabel, Command saveImage) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            FileFilter tfsFilter = new FileFilter("TFS", ".tfs", ".TFS");

            openFileDialog.MultiSelect = false;
            openFileDialog.Filters.Add(tfsFilter);
            openFileDialog.ShowDialog(control);

            if (!string.IsNullOrEmpty(openFileDialog.FileName)) {
                paletteDropdown.Visible = false;
                paletteDropdown.Items.Clear();
                paletteDropdownLabel.Visible = false;
                saveImage.Enabled = false;
                tfsReader.Read(openFileDialog.FileName);
                imageView.Image = tfsReader.RenderImage(0);

                if (tfsReader.GetPaletteCount() > 1) {
                    List<String> options = Enumerable.Range(0, tfsReader.GetPaletteCount()).Select(i => i.ToString()).ToList();
                    foreach (String option in options) {
                        paletteDropdown.Items.Add(option);
                    }
                    paletteDropdown.SelectedKey = "0";
                    paletteDropdown.Visible = true;
                    paletteDropdownLabel.Visible = true;
                }

                saveImage.Enabled = true;
            }
        }

        private void SaveImage(Control control, DropDown paletteDropdown) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            FileFilter saveFilter = new FileFilter("BMP", ".bmp");

            saveFileDialog.Filters.Add(new FileFilter("BMP", ".bmp"));
            saveFileDialog.Filters.Add(new FileFilter("JPEG", ".jpeg"));
            saveFileDialog.Filters.Add(new FileFilter("PNG", ".png"));
            saveFileDialog.ShowDialog(control);

            if (!string.IsNullOrEmpty(saveFileDialog.FileName)) {
                Bitmap bitmapToSave = tfsReader.RenderImage(int.Parse((string)paletteDropdown.SelectedKey));
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
                imageView.Image = tfsReader.RenderImage(int.Parse((string)paletteDropdown.SelectedKey));
            }
        }
    }
}
