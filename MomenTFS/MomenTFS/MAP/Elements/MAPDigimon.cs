using MomenTFS.Extensions;
using MomenTFS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.Elements
{
    public class MAPDigimon
    {
        public short Type { get; private set; }
        public short AIBehaviour { get; private set; }
        public SVector3 Position { get; private set; } 
        public SVector3 Rotation { get; private set; }
        public short AITrackingRange { get; private set; }
        public byte ScriptID { get; private set; }
        public short CurrentHP { get; private set; }
        public short CurrentMP { get; private set; }
        public short MaxHP { get; private set; }
        public short MaxMP { get; private set; }
        public short Offense { get; private set; }
        public short Defense { get; private set; }
        public short Speed { get; private set; }
        public short Brains { get; private set; }
        public short BitsDropped { get; private set; }
        public short ChargeMode { get; private set; }
        public short[] Moves { get; private set; }
        public short[] MoveChances { get; private set; }
        public SVector3 FleePosition { get; private set; }
        public short AISectionPositionCount { get; private set; }
        public short[] AISections { get; private set; }
        public SVector3[] AISectionsPositions { get; private set; }

        public MAPDigimon(Stream stream) {
            Type = stream.ReadShort();
            AIBehaviour = stream.ReadShort();
            Position = stream.ReadSVector3();
            Rotation = stream.ReadSVector3();
            AITrackingRange = stream.ReadShort();

            stream.Seek(2, SeekOrigin.Current);

            ScriptID = (byte)stream.ReadByte();

            stream.Seek(1, SeekOrigin.Current);

            CurrentHP = stream.ReadShort();
            CurrentMP = stream.ReadShort();
            MaxHP = stream.ReadShort();
            MaxMP = stream.ReadShort();
            Offense = stream.ReadShort();
            Defense = stream.ReadShort();
            Speed = stream.ReadShort();
            Brains = stream.ReadShort();
            BitsDropped = stream.ReadShort();
            ChargeMode = stream.ReadShort();

            stream.Seek(2, SeekOrigin.Current);

            Moves = new short[4];

            for (int i = 0; i < 4; ++i) {
                Moves[i] = stream.ReadShort();
            }

            MoveChances = new short[4];

            for (int i = 0; i < 4; ++i) {
                MoveChances[i] = stream.ReadShort();
            }

            FleePosition = stream.ReadSVector3();
            AISectionPositionCount = stream.ReadShort();
            AISections = new short[8];

            for (int i = 0; i < 8; ++i) {
                AISections[i] = stream.ReadShort();
            }

            AISectionsPositions = new SVector3[AISectionPositionCount];

            for (int i = 0; i < AISectionPositionCount; ++i) {
                AISectionsPositions[i] = stream.ReadSVector3(); 
            }
        }

    }
}
