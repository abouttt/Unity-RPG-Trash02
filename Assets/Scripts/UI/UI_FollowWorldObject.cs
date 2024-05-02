using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    public Transform Target
    {
        get => _target;
        set
        {
            _target = value;
            gameObject.SetActive(_target != null);
        }
    }

    [field: SerializeField]
    public Vector3 Offset { get; set; }

    private Transform _target;
    private RectTransform _rt;
    private Camera _mainCamera;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _rt.position = _mainCamera.WorldToScreenPoint(_target.position + Offset);
    }
}
