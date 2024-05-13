using UnityEngine;
using EnumType;

public class GameScene : BaseScene
{
    [field: SerializeField, Space(10)]
    public string SceneID { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 DefaultSpawnPosition { get; private set; }

    [field: SerializeField]
    public float DefaultSpawnYaw { get; private set; }

    protected override void Init()
    {
        if (Managers.Resource.ResourceCount == 0)
        {
            if (!Managers.Data.HasSaveData)
            {
                Managers.Game.IsDefaultSpawn = true;
            }

            Managers.Scene.LoadScene(Managers.Scene.CurrentScene.SceneType);
        }
        else
        {
            base.Init();
            Player.Init();
            InstantiatePackage("UIPackage_Game.prefab");
        }
    }

    private void Start()
    {
        Managers.Game.IsDefaultSpawn = false;
        Managers.Game.IsPortalSpawn = false;
        Managers.Input.CursorLocked = true;
        Managers.Quest.Load();
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);
        if (!Managers.Data.HasSaveData)
        {
            Player.Status.Gold += 10000;
            Player.Status.SkillPoint += 5;
        }
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
    }
}
