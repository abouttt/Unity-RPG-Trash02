using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_MinimapFixed : UI_Base, IPointerMoveHandler, IPointerExitHandler
{
    enum GameObjects
    {
        MinimapIconName,
    }

    enum Texts
    {
        NameText,
    }

    enum RawImages
    {
        MinimapImage,
    }

    enum Cameras
    {
        MinimapCamera,
    }

    [SerializeField]
    private float _height;

    [SerializeField, Tooltip("Distance from mouse")]
    private Vector2 _offset;

    private MinimapIcon _currentMinimapIcon;
    private RectTransform _rt;
    private GameObject _mainCamera;

    protected override void Init()
    {
        Managers.UI.Register<UI_MinimapFixed>(this);

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        Bind<RawImage>(typeof(RawImages));
        Bind<Camera>(typeof(Cameras));

        _rt = GetObject((int)GameObjects.MinimapIconName).GetComponent<RectTransform>();
        _mainCamera = Camera.main.gameObject;
    }

    private void Start()
    {
        GetObject((int)GameObjects.MinimapIconName).SetActive(false);
    }

    private void LateUpdate()
    {
        var position = Player.Transform.position;
        position.y = _height;
        var euler = _mainCamera.transform.rotation.eulerAngles;
        euler.x = 90f;
        euler.z = 0f;
        Get<Camera>((int)Cameras.MinimapCamera).transform.SetPositionAndRotation(position, Quaternion.Euler(euler));
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Get<RawImage>((int)RawImages.MinimapImage).rectTransform,
            eventData.position, eventData.enterEventCamera, out var cursor))
        {
            var texture = Get<RawImage>((int)RawImages.MinimapImage).texture;
            var rect = Get<RawImage>((int)RawImages.MinimapImage).rectTransform.rect;

            float coordX = Mathf.Clamp(0, ((cursor.x - rect.x) * texture.width) / rect.width, texture.width);
            float coordY = Mathf.Clamp(0, ((cursor.y - rect.y) * texture.height) / rect.height, texture.height);

            float calX = coordX / texture.width;
            float calY = coordY / texture.height;

            cursor = new Vector2(calX, calY);

            RaycastToWorld(cursor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetObject((int)GameObjects.MinimapIconName).SetActive(false);
    }

    private void RaycastToWorld(Vector2 vec)
    {
        var minimapCamera = Get<Camera>((int)Cameras.MinimapCamera);
        var mapRay = minimapCamera.ScreenPointToRay(new Vector2(vec.x * minimapCamera.pixelWidth, vec.y * minimapCamera.pixelHeight));
        if (Physics.Raycast(mapRay, out var miniMapHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Minimap")))
        {
            if (_currentMinimapIcon != null && _currentMinimapIcon.gameObject == miniMapHit.collider.gameObject)
            {
                return;
            }

            GetObject((int)GameObjects.MinimapIconName).SetActive(true);
            GetText((int)Texts.NameText).text = miniMapHit.collider.gameObject.GetComponent<MinimapIcon>().IconName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
            SetPosition(Mouse.current.position.ReadValue());
        }
        else
        {
            GetObject((int)GameObjects.MinimapIconName).SetActive(false);
        }
    }

    private void SetPosition(Vector3 position)
    {
        var nextPosition = new Vector3
        {
            x = position.x + (_rt.rect.width * 0.5f) + _offset.x,
            y = position.y + (_rt.rect.height * 0.5f) + _offset.y
        };

        _rt.position = nextPosition;
    }
}
