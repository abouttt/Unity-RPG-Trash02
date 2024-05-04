using UnityEngine;
using EnumType;

public class UI_ItemInventoryTab : BaseMonoBehaviour
{
    [field: SerializeField]
    public ItemType TabType { get; private set; }
    public RectTransform SlotsRT { get; private set; }

    private void Awake()
    {
        SlotsRT = GetComponent<RectTransform>();
    }
}
