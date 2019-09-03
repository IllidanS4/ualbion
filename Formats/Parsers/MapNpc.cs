﻿using System;
using System.IO;

namespace UAlbion.Formats.Parsers
{
    public class MapNpc
    {
        [Flags]
        public enum NpcFlags : byte
        {
            Unk0 = 1 << 0,
            Unk1 = 1 << 1,
            Unk2 = 1 << 2,
            Unk3 = 1 << 3,
            Unk4 = 1 << 4,
            Unk5 = 1 << 5,
            Unk6 = 1 << 6,
            Unk7 = 1 << 7,
        }

        [Flags]
        public enum MovementType : byte
        {
            Random1 = 1,
            Random2 = 2,
            FollowParty = 4,
            Stationary = 8,

            RandomMask = 3,
        }

        public struct Waypoint
        {
            public byte X;
            public byte Y;
            public override string ToString() => $"({X}, {Y})";
        }

        public byte Id { get; set; }
        public byte Sound { get; set; }
        public ushort? EventNumber { get; set; }
        public ushort ObjectNumber { get; set; }
        public NpcFlags Flags { get; set; } // 1=Dialogue, 2=AutoAttack, 11=ReturnMsg
        public MovementType Movement { get; set; }
        public ushort Unk8 { get; set; }
        public int Unk9 { get; set; }
        public Waypoint[] Waypoints { get; set; }

        public static MapNpc Load(BinaryReader br)
        {
            var npc = new MapNpc();
            npc.Id = br.ReadByte(); // +0
            npc.Sound = br.ReadByte(); // +1
            npc.EventNumber = br.ReadUInt16(); // +2
            if (npc.EventNumber == 0xffff) npc.EventNumber = null;

            npc.ObjectNumber = (ushort)(br.ReadUInt16() - 1); // +4
            npc.Flags = (NpcFlags)br.ReadByte(); // +6 // Combine this & MovementType ?
            npc.Movement = (MovementType)br.ReadByte(); // +7
            npc.Unk8 = br.ReadByte(); // +8
            npc.Unk9 = br.ReadByte(); // +9
            return npc;
        }

        public void LoadWaypoints(BinaryReader br)
        {
            if ((Movement & MovementType.RandomMask) != 0)
            {
                var wp = new Waypoint();
                byte x = br.ReadByte();
                byte y = br.ReadByte();
                wp.X = (byte)(x - 1);
                wp.Y = (byte)(y - 1);
                Waypoints = new[] { wp };
            }
            else
            {
                Waypoints = new Waypoint[0x480];
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    byte x = br.ReadByte();
                    byte y = br.ReadByte();
                    Waypoints[i].X = (byte)(x - 1);
                    Waypoints[i].Y = (byte)(y - 1);
                }
            }
        }

        public override string ToString() => $"Npc{Id} Obj{ObjectNumber} F={Flags:x} M{Movement} Amount={Unk8} Unk9={Unk9} S{Sound} E{EventNumber}";
    }
}