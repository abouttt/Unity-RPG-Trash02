using UnityEngine;

public class Player : BaseMonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Transform Transform { get; private set; }
    public static Collider Collider { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCamera Camera { get; private set; }
    public static PlayerCombat Combat { get; private set; }
    public static PlayerStatus Status { get; private set; }
    public static InteractionDetector InteractionDetector { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }
    public static EquipmentInventory EquipmentInventory { get; private set; }
    public static QuickInventory QuickInventory { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Transform = transform;
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCamera>();
        Combat = GetComponent<PlayerCombat>();
        Status = GetComponent<PlayerStatus>();
        InteractionDetector = GetComponentInChildren<InteractionDetector>();
        ItemInventory = GetComponent<ItemInventory>();
        EquipmentInventory = GetComponent<EquipmentInventory>();
        QuickInventory = GetComponent<QuickInventory>();
    }

    private void Start()
    {
        var go = Managers.Resource.Instantiate("MinimapIcon.prefab", transform);
        go.GetComponent<MinimapIcon>().Setup("PlayerMinimapIcon.sprite", "플레이어", 1.2f);
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
