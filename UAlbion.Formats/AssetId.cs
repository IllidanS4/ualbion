﻿namespace UAlbion.Formats
{
    public enum AssetType
    {
        MapData,
        IconData,
        IconGraphics,
        Palette,
        PaletteNull,
        Slab,
        BigPartyGraphics,
        SmallPartyGraphics,
        LabData,
        Wall3D,
        Object3D,
        Overlay3D,
        Floor3D,
        BigNpcGraphics,
        BackgroundGraphics,
        Font,
        BlockList,
        PartyCharacterData,
        SmallPortrait,
        SystemTexts,
        EventSet,
        EventTexts,
        MapTexts,
        ItemList,
        ItemNames,
        ItemGraphics,
        FullBodyPicture,
        Automap,
        AutomapGraphics,
        Song,
        Sample,
        WaveLibrary,
        Unnamed2, // Unused
        ChestData,
        MerchantData,
        NpcCharacterData,
        MonsterGroup,
        MonsterCharacter,
        MonsterGraphics,
        CombatBackground,
        CombatGraphics,
        TacticalIcon,
        SpellData,
        SmallNpcGraphics,
        Flic,
        Dictionary,
        Script,
        Picture,
        TransparencyTables,
    }

    enum AssetLocation
    {
        Base,
        BaseRaw, // Not in an XLD
        Localised,
        LocalisedRaw, // Not in an XLD
        Initial,
        Current
    }
}