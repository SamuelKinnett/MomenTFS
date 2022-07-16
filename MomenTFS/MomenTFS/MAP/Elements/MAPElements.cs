using MomenTFS.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.Elements
{
    public class MAPElements
    {
        public SpawnData[] SpawnPoints { get; private set; }
        public short[] WarpTargetMaps;
        public short[] WarpTargetPoints;
        public short NumberOfDigimon;
        public MAPDigimon[] Digimon;

        public MAPElements(Stream stream) {
            short[] SpawnXPositions = new short[10];
            short[] SpawnYPositions = new short[10];
            short[] SpawnZPositions = new short[10];
            short[] SpawnRotations = new short[10];

            for (int i = 0; i < 10; ++i) {
                SpawnXPositions[i] = stream.ReadShort();
            }

            for (int i = 0; i < 10; ++i) {
                SpawnYPositions[i] = stream.ReadShort();
            }

            for (int i = 0; i < 10; ++i) {
                SpawnZPositions[i] = stream.ReadShort();
            }

            for (int i = 0; i < 10; ++i) {
                SpawnRotations[i] = stream.ReadShort();
            }

            SpawnPoints = new SpawnData[10];

            for (int i = 0; i < 10; ++i) {
                SpawnPoints[i] = new SpawnData(
                    SpawnXPositions[i], SpawnYPositions[i], SpawnZPositions[i], SpawnRotations[i]);
            }

            WarpTargetMaps = new short[10];
            WarpTargetPoints = new short[10];

            for (int i = 0; i < 10; ++i) {
                WarpTargetMaps[i] = stream.ReadShort();
            }

            for (int i = 0; i < 10; ++i) {
                WarpTargetPoints[i] = stream.ReadShort();
            }

            NumberOfDigimon = Math.Max((short) 0, stream.ReadShort());
            Digimon = new MAPDigimon[NumberOfDigimon];

            for (int i = 0; i < NumberOfDigimon; ++i) {
                Digimon[i] = new MAPDigimon(stream);
            }
        }
    }
}
