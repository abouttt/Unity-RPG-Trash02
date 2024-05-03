using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public abstract class UI_BaseSlot : UI_Base,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private enum Images
    {
        SlotImage,
        TempImage,
    }

    public static bool IsDragging { get; private set; }

    [field: SerializeField]
    public SlotType SlotType { get; private set; }

    public object ObjectRef { get; private set; }
    public bool HasObject => ObjectRef != null;

    [field: SerializeField]
    protected bool CanDrag = true;

    protected bool IsPointerDown = false;

    protected override void Init()
    {
        BindImage(typeof(Images));
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    protected void SetObject(object obj, Sprite image)
    {
        if (obj == null)
        {
            return;
        }

        ObjectRef = obj;
        GetImage((int)Images.SlotImage).sprite = image;
        GetImage((int)Images.TempImage).sprite = image;
        GetImage((int)Images.SlotImage).gameObject.SetActive(true);
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    protected virtual void Clear()
    {
        ObjectRef = null;
        GetImage((int)Images.SlotImage).sprite = null;
        GetImage((int)Images.TempImage).sprite = null;
        GetImage((int)Images.SlotImage).gameObject.SetActive(false);
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if ((eventData.button != PointerEventData.InputButton.Left) || !HasObject || !CanDrag)
        {
            eventData.pointerDrag = null;
            return;
        }

        IsDragging = true;

        GetImage((int)Images.TempImage).gameObject.SetActive(true);
        GetImage((int)Images.TempImage).transform.SetParent(Managers.UI.Get<UI_TopCanvas>().transform);
        GetImage((int)Images.TempImage).transform.SetAsLastSibling();
        GetImage((int)Images.SlotImage).color -= new Color(0f, 0f, 0f, 0.7f);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).rectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;

        GetImage((int)Images.TempImage).gameObject.SetActive(false);
        GetImage((int)Images.TempImage).transform.SetParent(transform);
        GetImage((int)Images.TempImage).rectTransform.position = transform.position;
        GetImage((int)Images.SlotImage).color = Color.white;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if ((eventData.button != PointerEventData.InputButton.Left) || !HasObject)
        {
            return;
        }

        IsPointerDown = true;
    }

    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);
    public abstract void OnPointerUp(PointerEventData eventData);

    protected bool CanPointerUp()
    {
        if (!IsPointerDown)
        {
            return false;
        }

        IsPointerDown = false;

        if (IsDragging)
        {
            return false;
        }

        return true;
    }
}
