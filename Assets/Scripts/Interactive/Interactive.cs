using UnityEngine;

public abstract class Interactive : BaseMonoBehaviour
{
    public bool IsDetected { get; set; }    // 감지 되었는지
    public bool IsInteracted { get; set; }  // 상호작용 중인지

    [field: SerializeField]
    public string InteractionMessage { get; protected set; }

    [field: SerializeField]
    public Vector3 InteractionKeyGuidePos { get; protected set; }

    [field: SerializeField]
    public float InteractionInputTime { get; protected set; }

    [field: SerializeField]
    public bool CanInteraction { get; protected set; } = true;

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactive");
    }

    public abstract void Interaction();

    protected void InstantiateMinimapIcon(string spriteName, string iconName, float scale = 1f)
    {
        var go = Managers.Resource.Instantiate("MinimapIcon.prefab", transform);
        go.GetComponent<MinimapIcon>().Setup(spriteName, iconName, scale);
    }

    private void OnDrawGizmosSelected()
    {
        // InteractionKeyGuidePos 위치 시각화
        Gizmos.DrawWireSphere(transform.position + InteractionKeyGuidePos, 0.1f);
    }
}
