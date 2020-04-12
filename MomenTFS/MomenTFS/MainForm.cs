using System;
using Eto.Forms;
using Eto.Drawing;
using MomenTFS.Reader;
using System.Collections.Generic;
using System.Linq;

namespace MomenTFS
{
    public partial class MainForm : Form
    {
        TFSReader tfsReader;

        public MainForm()
        {
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

            // create a few commands that can be used for the menu and toolbar
            var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

            var loadTFS = new Command { MenuText = "Load TFS", ToolBarText = "Load TFS" };
            loadTFS.Executed += (sender, e) => OpenTFS(this, imageView, paletteDropdown, paletteDropdownLabel);

            // create menu
            Menu = new MenuBar {
                Items =
                {
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = { clickMe } },
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
            ToolBar = new ToolBar { Items = { clickMe, loadTFS } };
        }

        private void OpenTFS(Control control, ImageView imageView, DropDown paletteDropdown, Label paletteDropdownLabel) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            FileFilter tfsFilter = new FileFilter("TFS", ".tfs", ".TFS");

            openFileDialog.MultiSelect = false;
            openFileDialog.Filters.Add(tfsFilter);
            openFileDialog.ShowDialog(control);

            if (!string.IsNullOrEmpty(openFileDialog.FileName)) {
                paletteDropdown.Visible = false;
                paletteDropdown.Items.Clear();
                paletteDropdownLabel.Visible = false;
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
            }
        }

        private void RenderTFS(DropDown paletteDropdown, ImageView imageView) {
            if (paletteDropdown.Visible) {
                imageView.Image = tfsReader.RenderImage(int.Parse((string)paletteDropdown.SelectedKey));
            }
        }
    }
}
