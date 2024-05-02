using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static PoolManager Pool => GetInstance._pool;
    public static ResourceManager Resource => GetInstance._resource;
    public static SceneManagerEx Scene => GetInstance._scene;
    public static SoundManager Sound => GetInstance._sound;
    public static UIManager UI => GetInstance._ui;

    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    private static bool s_init = false;

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    public static void Init()
    {
        if (s_init)
        {
            return;
        }

        Pool.Init();
        Sound.Init();
        UI.Init();

        s_init = true;
    }

    public static void Clear()
    {
        if (!s_init)
        {
            return;
        }

        Pool.Clear();
        Sound.Clear();
        UI.Clear();

        s_init = false;
    }
}
