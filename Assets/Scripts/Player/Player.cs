using UnityEngine;

public class Player : BaseMonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Transform Transform { get; private set; }
    public static Collider Collider { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerInputController Input{ get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCamera Camera { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Transform = transform;
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        Input = GetComponent<PlayerInputController>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCamera>();
        ItemInventory = GetComponent<ItemInventory>();
    }

    public static void Init()
    {
        if (GameObject != null)
        {
            return;
        }

        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage.prefab");
        var playerPrefab = playerPackagePrefab.FindChild("Player");
        GetPositionAndRotationYaw(out var position, out var yaw);
        playerPrefab.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));

        var playerPackage = Instantiate(playerPackagePrefab);
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);

        playerPrefab.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }

    private static void GetPositionAndRotationYaw(out Vector3 position, out float yaw)
    {
        position = Vector3.zero;
        yaw = 0f;

        if (Managers.Game.IsDefaultSpawn)
        {
            var gameScene = Managers.Scene.CurrentScene as GameScene;
            position = gameScene.DefaultSpawnPosition;
            yaw = gameScene.DefaultSpawnYaw;
        }
    }
}
