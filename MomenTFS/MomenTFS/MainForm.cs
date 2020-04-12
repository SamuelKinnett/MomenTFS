using System;
using Eto.Forms;
using Eto.Drawing;
using MomenTFS.Reader;
using System.Collections.Generic;

namespace MomenTFS
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Title = "MomenTFS";
            ClientSize = new Size(600, 480);

            var imageView = new ImageView();
            var scrollable = new Scrollable {
                Content = imageView,
                ExpandContentWidth = false,
                ExpandContentHeight = false
            };

            scrollable.Content = imageView;

            var layout = new DynamicLayout();
            layout.BeginVertical();
            
            layout.AddRow(new Label { Text = "Preview", HorizontalAlign = HorizontalAlign.Left });
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
            loadTFS.Executed += (sender, e) => OpenTFS(this, imageView);

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

        private void OpenTFS(Control control, ImageView imageView) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            FileFilter tfsFilter = new FileFilter("TFS", ".tfs", ".TFS");
            TFSReader tfsReader = new TFSReader();

            openFileDialog.MultiSelect = false;
            openFileDialog.Filters.Add(tfsFilter);
            openFileDialog.ShowDialog(control);

            if (!string.IsNullOrEmpty(openFileDialog.FileName)) {
                tfsReader.Read(openFileDialog.FileName);
                imageView.Image = tfsReader.RenderImage(0);
            }
        }
    }
}
