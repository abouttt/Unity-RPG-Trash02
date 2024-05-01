namespace EnumType
{
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
        GameScene,
    }

    public enum AddressableLabel
    {
        Default,
        Game_Prefab,
        Game_UI,
    }
}
