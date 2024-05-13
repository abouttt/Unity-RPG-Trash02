namespace EnumType
{
    public enum MonsterState
    {
        Idle,
        Tracking,
        Restore,
        Attack,
        Damaged,
        Stunned,
        Death,
    }

    public enum Category
    {
        Scene,
        Item,
        Skill,
        Quest,
        NPC,
    }

    public enum QuestState
    {
        Inactive,
        Active,
        Completable,
        Complete,
    }

    public enum SkillType
    {
        Active,
        Passive,
    }

    public enum SlotType
    {
        Item,
        Equipment,
        Skill,
        Quick,
        Shop,
        Reward,
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
        DungeonScene,
    }

    public enum AddressableLabel
    {
        Default,
        MainMenu_UI,
        Game_Prefab,
        Game_UI,
    }
}
