﻿using SerdesNet;
using UAlbion.Api;
using UAlbion.Formats.AssetIds;

namespace UAlbion.Formats.MapEvents
{
    [Event("inv:chest", "Opens the inventory screen for the given chest")]
    public class ChestEvent : MapEvent, ILockedInventoryEvent
    {
        public static ChestEvent Parse(string[] args)
        {
            return new ChestEvent(AssetType.SystemText, 0)
            {
                ChestId = (ChestId)int.Parse(args[1]),
                PickDifficulty = args.Length > 2 ? byte.Parse(args[2]) : (byte)0,
                InitialTextId =  args.Length > 3 ? byte.Parse(args[3]) : (byte)255,
                UnlockedTextId = args.Length > 4 ? byte.Parse(args[4]) : (byte)255,
                KeyItemId = args.Length > 5 ? (ItemId?)int.Parse(args[5]) : null,
            };
        }

        ChestEvent(AssetType textType, ushort textSourceId)
        {
            TextType = textType;
            TextSourceId = textSourceId;
        }

        public static ChestEvent Serdes(ChestEvent e, ISerializer s, AssetType textType, ushort textSourceId)
        {
            e ??= new ChestEvent(textType, textSourceId);
            s.Begin();
            e.PickDifficulty = s.UInt8(nameof(PickDifficulty), e.PickDifficulty);
            e.KeyItemId = s.TransformEnumU16(nameof(KeyItemId), e.KeyItemId, StoreIncrementedNullZero<ItemId>.Instance);
            e.UnlockedTextId = s.UInt8(nameof(UnlockedTextId), e.UnlockedTextId);
            e.InitialTextId = s.UInt8(nameof(InitialTextId), e.InitialTextId);
            e.ChestId = s.EnumU16(nameof(ChestId), e.ChestId);
            s.End();
            return e;
        }

        public override MapEventType EventType => MapEventType.Chest;
        public InventoryMode Mode => InventoryMode.Chest;
        public ushort Submode => (ushort)ChestId;
        public ChestId ChestId { get; private set; }
        public byte PickDifficulty { get; private set; }
        public ItemId? KeyItemId { get; private set; }
        public byte InitialTextId { get; private set; }
        public byte UnlockedTextId { get; private set; }
        public AssetType TextType { get; }
        public ushort TextSourceId { get; }
        public override string ToString() => $"inv:chest {ChestId} {PickDifficulty}% Initial:{InitialTextId} Unlocked:{UnlockedTextId} Key:{KeyItemId}";
    }
}
