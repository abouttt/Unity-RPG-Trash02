using UnityEngine;
using EnumType;

public class Portal : Interactive
{
    [SerializeField]
    private SceneType _loadScene;

    [SerializeField]
    private Vector3 _spawnPosition;

    [SerializeField]
    private float _spawnYaw;

    private void Start()
    {
        Util.InstantiateMinimapIcon("PortalMinimapIcon.sprite", "��Ż", transform);
    }

    public override void Interaction()
    {
        Managers.Game.IsPortalSpawn = true;
        Managers.Game.PortalSpawnPosition = _spawnPosition;
        Managers.Game.PortalSpawnYaw = _spawnYaw;
        Managers.Scene.LoadScene(_loadScene);
    }
}
