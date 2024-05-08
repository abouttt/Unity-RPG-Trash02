namespace EnumType
{
    public enum SlotType
    {
        Item,
        Equipment,
        Quick,
        Shop,
    }

    public enum EquipmentType
    {
        Helmet,
        Chest,
        Pants,
        Boots,
        Weapon,
        Shield,
    }

    public enum ItemType
    {
        Equipment,
        Consumable,
        Etc,
    }

    public enum ItemQuality
    {
        Low,
        Normal,
        High,
    }

    public enum UIType
    {
        Subitem,
        Background,
        Auto,
        Fixed,
        Popup,
        Top = 1000,
    }

    public enum SoundType
    {
        BGM,
        Effect,
        UI,
    }

    public enum SceneType
    {
        Unknown,
        LoadingScene,
        MainMenuScene,
        VillageScene,
    }

    public enum AddressableLabel
    {
        Default,
        MainMenu_UI,
        Game_Prefab,
        Game_UI,
    }
}
