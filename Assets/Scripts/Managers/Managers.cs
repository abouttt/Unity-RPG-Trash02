using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static CooldownManager Cooldown => GetInstance._cooldown;
    public static DataManager Data => GetInstance._data;
    public static GameManager Game => GetInstance._game;
    public static InputManager Input => GetInstance._input;
    public static PoolManager Pool => GetInstance._pool;
    public static QuestManager Quest => GetInstance._quest;
    public static ResourceManager Resource => GetInstance._resource;
    public static SceneManagerEx Scene => GetInstance._scene;
    public static SoundManager Sound => GetInstance._sound;
    public static UIManager UI => GetInstance._ui;

    private readonly CooldownManager _cooldown = new();
    private readonly DataManager _data = new();
    private readonly GameManager _game = new();
    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly QuestManager _quest = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    private static bool s_init = false;

    private void LateUpdate()
    {
        _cooldown.Cooling();
    }

    public static void Init()
    {
        if (s_init)
        {
            return;
        }

        Data.Init();
        Input.Init();
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

        Cooldown.Clear();
        Input.Clear();
        Pool.Clear();
        Quest.Clear();
        Sound.Clear();
        UI.Clear();

        s_init = false;
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
