using Eto.Forms;
using MomenTFS.MAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.Forms {
    public class MapDetailsPage : TabPage {
        public TextArea Details { get; }
        public MAPData MapData {
            get { return mapData; }
            set { UpdateMapData(value); }
        }

        private readonly DynamicLayout layout = new DynamicLayout();
        private readonly NumericStepper cameraPositionXStepper = new NumericStepper();
        private readonly NumericStepper cameraPositionYStepper = new NumericStepper();
        private readonly NumericStepper cameraPositionZStepper = new NumericStepper();
        private readonly NumericStepper cameraTranslationXStepper = new NumericStepper();
        private readonly NumericStepper cameraTranslationYStepper = new NumericStepper();
        private readonly NumericStepper cameraTranslationZStepper = new NumericStepper();
        private readonly NumericStepper zoomStepper = new NumericStepper();

        private MAPData mapData;

        public MapDetailsPage() {
            this.Text = "Details";
            Details = new TextArea { ReadOnly = true, };
            layout.BeginVertical();

            layout.BeginHorizontal();
            layout.Add(new Label() { Text = "Camera Position" });
            layout.BeginVertical();
            layout.BeginHorizontal();
            layout.Add(cameraPositionXStepper);
            layout.Add(cameraPositionYStepper);
            layout.Add(cameraPositionZStepper);
            layout.EndHorizontal();
            layout.EndVertical();
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(new Label() { Text = "Camera Translation" });
            layout.BeginVertical();
            layout.BeginHorizontal();
            layout.Add(cameraTranslationXStepper);
            layout.Add(cameraTranslationYStepper);
            layout.Add(cameraTranslationZStepper);
            layout.EndHorizontal();
            layout.EndVertical();
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(new Label() { Text = "Zoom" });
            layout.Add(zoomStepper);
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(new Label() { Text = "Details" });
            layout.Add(Details, true, true);
            layout.EndHorizontal();

            layout.EndVertical();
            this.Content = layout;
        }

        private void UpdateMapData(MAPData newMapData) {
            mapData = newMapData;
            cameraPositionXStepper.Value = mapData.Settings.CameraOrigin.X;
            cameraPositionYStepper.Value = mapData.Settings.CameraOrigin.Y;
            cameraPositionZStepper.Value = mapData.Settings.CameraOrigin.Z;

            cameraTranslationXStepper.Value = mapData.Settings.CameraTranslation.X;
            cameraTranslationYStepper.Value = mapData.Settings.CameraTranslation.Y;
            cameraTranslationZStepper.Value = mapData.Settings.CameraTranslation.Z;

            zoomStepper.Value = mapData.Settings.Zoom;

            Details.Text = getDetailsText();
        }

        public MAPData GetDataToPatch() {
            MAPData dataToPatch = new MAPData();
            dataToPatch.Settings = mapData.Settings;
            dataToPatch.TIMImages = mapData.TIMImages;
            dataToPatch.Objects = mapData.Objects;
            dataToPatch.Elements = mapData.Elements;
            dataToPatch.Collision = mapData.Collision;

            dataToPatch.Settings.CameraOrigin.X = (int)cameraPositionXStepper.Value;
            dataToPatch.Settings.CameraOrigin.Y = (int)cameraPositionYStepper.Value;
            dataToPatch.Settings.CameraOrigin.Z = (int)cameraPositionZStepper.Value;

            dataToPatch.Settings.CameraTranslation.X = (int)cameraTranslationXStepper.Value;
            dataToPatch.Settings.CameraTranslation.Y = (int)cameraTranslationYStepper.Value;
            dataToPatch.Settings.CameraTranslation.Z = (int)cameraTranslationZStepper.Value;

            dataToPatch.Settings.Zoom = (ushort)zoomStepper.Value;

            return dataToPatch;
        }

        private string getDetailsText() {

            var detailsText = $"Camera Origin: {mapData.Settings.CameraOrigin}\n" +
                $"Camera Translation: {mapData.Settings.CameraTranslation}\n";

            for (int i = 0; i < 3; ++i) {
                detailsText += $"Light {i}:\n\t{mapData.Settings.Lights[i]}\n";
            }
            double zoom = 10000f / mapData.Settings.Zoom;
            //double fieldOfView = 2 * Math.Atan(1 / (float) mapData.Settings.Zoom);
            //double fieldOfView = 2 * Math.Atan(Math.Sqrt(Math.Pow(mapData.Settings.MapTileWidth, 2) + Math.Pow(mapData.Settings.MapTileHeight, 2)) / zoom);

            detailsText += $"Zoom: {mapData.Settings.Zoom}\n" +
                //$"FOV: {fieldOfView}\n" +
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
    }
}
