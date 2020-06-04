using Eto.Drawing;
using Eto.Forms;
using MomenTFS.MAP.Objects;
using MomenTFS.Objects;
using MomenTFS.TIM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MomenTFS.Forms
{
    public class MAPObjectsPage
    {
        private TabPage page;
        private TabPage mapObjectsPage;
        private TabPage mapObjectInstancesPage;

        public MAPObjectsPage(TabControl tabControl) {
            page = new TabPage { Text = "Objects" };
            tabControl.Pages.Add(page);

            var mapObjectTabControl = new TabControl();
            mapObjectsPage = new TabPage { Text = "Objects" };
            mapObjectInstancesPage = new TabPage { Text = "Instances" };

            mapObjectTabControl.Pages.Add(mapObjectsPage);
            mapObjectTabControl.Pages.Add(mapObjectInstancesPage);

            page.Content = mapObjectTabControl;
        }

        public void Update(RoomData roomData) {
            var mapObjectsList = new DynamicLayout();
            mapObjectsList.BeginVertical();
            for (int i = 0; i < roomData.MAPData.Objects.Objects.Length; ++i) {
                var mapObject = roomData.MAPData.Objects.Objects[i];

                mapObjectsList.BeginHorizontal();
                mapObjectsList.BeginVertical();
                mapObjectsList.AddRow(new TextArea { Text = i.ToString() });
                mapObjectsList.EndVertical();
                mapObjectsList.BeginVertical();
                mapObjectsList.AddRow(new Scrollable {
                    Content = new ImageView {
                        Image = GetBitmapForObject(mapObject, roomData.MAPData.TIMImages.ToArray())
                    },
                    ExpandContentWidth = false,
                    ExpandContentHeight = false
                });
                mapObjectsList.EndVertical();
                mapObjectsList.BeginVertical();
                mapObjectsList.AddRow(new TextArea {
                    Text = mapObject.ToString(),
                    ReadOnly = true,
                    Size = new Size { Width = -1, Height = 200 }
                });
                mapObjectsList.EndVertical();
                mapObjectsList.EndHorizontal();
            }
            mapObjectsList.EndVertical();
            var mapObjectsScrollable = new Scrollable {
                Content = mapObjectsList,
                ExpandContentWidth = false,
                ExpandContentHeight = false
            };
            mapObjectsPage.Content = mapObjectsScrollable;
        }

        private Bitmap GetBitmapForObject(MAPObject mapObject, TIMImage[] images) {
            var imageIndex = (int)Math.Floor(mapObject.SpritesheetX / 256f);
            if (imageIndex >= images.Length) {
                return null;
            }

            TIMImage image = images[imageIndex];
            System.Drawing.Color[,] imageData = image.GetBitmap();
            List<Color> convertedBitmapData = new List<Color>();

            int imageX = mapObject.SpritesheetX % 256;
            int imageY = mapObject.SpritesheetY % 256;

            for (int y = imageY; y < imageY + mapObject.Height; ++y) {
                for (int x = imageX; x < imageX + mapObject.Width; ++x) {
                    System.Drawing.Color currentColor = imageData[x, y];
                    convertedBitmapData.Add(
                        new Color(
                            currentColor.R / 255f,
                            currentColor.G / 255f,
                            currentColor.B / 255f));
                }
            }

            return new Bitmap(
                mapObject.Width,
                mapObject.Height,
                PixelFormat.Format32bppRgba,
                convertedBitmapData);
        }
    }
}
