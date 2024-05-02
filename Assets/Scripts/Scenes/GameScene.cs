using UnityEngine;

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
            Managers.Game.IsDefaultSpawn = true;
            Managers.Scene.LoadScene(Managers.Scene.CurrentScene.SceneType);
        }
        else
        {
            base.Init();
            Player.Init();
            var uiPackage = Managers.Resource.Instantiate("UIPackage.prefab");
            uiPackage.transform.DetachChildren();
            Destroy(uiPackage);
        }
    }

    private void Start()
    {
        Managers.Game.IsDefaultSpawn = false;
        Player.Input.CursorLocked = true;
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
    }
}
