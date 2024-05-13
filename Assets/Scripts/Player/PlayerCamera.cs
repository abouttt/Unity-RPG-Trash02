using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Newtonsoft.Json.Linq;
using Structs;

public class PlayerCamera : BaseMonoBehaviour, ISavable
{
    public static string SaveKey => "SaveCamera";

    public Transform LockedTarget
    {
        get => _stateDrivenCamera.LookAt;
        set
        {
            if (IsLockOn && value == null)
            {
                LockedTarget.GetComponent<LockOnTarget>().IsLockOn = false;
            }

            IsLockOn = value != null;
            _stateDrivenCamera.LookAt = value;
            _stateDrivenCameraAnimator.SetBool(_animIDLockOn, IsLockOn);
            _lockOnTargetImage.FollowWorldObject.Target = value;

            if (IsLockOn)
            {
                value.GetComponent<LockOnTarget>().IsLockOn = true;
                _lockOnTargetImage.FollowWorldObject.Offset = value.position - value.GetComponent<Collider>().bounds.center;
            }
        }
    }

    public bool IsLockOn { get; private set; }

    [Header("[Rotate]")]
    [SerializeField]
    private GameObject _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    [Space(10)]
    [Header("[Lock On Target]")]
    [SerializeField]
    private CinemachineStateDrivenCamera _stateDrivenCamera;

    [SerializeField]
    private float _viewRadius;

    [Range(0, 360)]
    [SerializeField]
    private float _viewAngle;

    [SerializeField]
    private LayerMask _targetMask;

    [SerializeField]
    private LayerMask _obstacleMask;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private readonly int _animIDLockOn = Animator.StringToHash("LockOn");
    private readonly float _threshold = 0.01f;

    private GameObject _mainCamera;
    private Animator _stateDrivenCameraAnimator;
    private CinemachineComposer _targetComposer;
    private UI_LockOnTargetImage _lockOnTargetImage;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _stateDrivenCameraAnimator = _stateDrivenCamera.GetComponent<Animator>();
        foreach (CinemachineVirtualCamera child in _stateDrivenCamera.ChildCameras)
        {
            _targetComposer = child.GetCinemachineComponent<CinemachineComposer>();
            if (_targetComposer != null)
            {
                break;
            }
        }

        Managers.Input.GetAction("LockOn").performed += FindTargetOrReset;

        if (!Managers.Game.IsDefaultSpawn && !Managers.Game.IsPortalSpawn)
        {
            Load();
        }
    }

    private void Start()
    {
        var go = Managers.Resource.Instantiate("UI_LockOnTargetImage.prefab");
        _lockOnTargetImage = go.GetComponent<UI_LockOnTargetImage>();

        _cinemachineTargetPitch = _cinemachineCameraTarget.transform.rotation.eulerAngles.x;
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (IsLockOn)
        {
            CalcTrackedObjectOffset();
            CanLockable();
        }
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();
        var vector3SaveData = new Vector3SaveData(_cinemachineCameraTarget.transform.rotation.eulerAngles);
        saveData.Add(JObject.FromObject(vector3SaveData));
        return saveData;
    }

    private void CameraRotation()
    {
        if (IsLockOn)
        {
            _cinemachineCameraTarget.transform.rotation = _mainCamera.transform.rotation;
            var eulerAngles = _cinemachineCameraTarget.transform.eulerAngles;
            _cinemachineTargetPitch = eulerAngles.x;
            _cinemachineTargetYaw = eulerAngles.y;
        }
        else
        {
            var look = Managers.Input.Look;
            if (look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += look.x * _sensitivity;
                _cinemachineTargetPitch += look.y * _sensitivity;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }
    }

    private void FindLockableTarget()
    {
        float shortestAngle = Mathf.Infinity;
        Transform finalTarget = null;

        var targets = Physics.OverlapSphere(_mainCamera.transform.position, _viewRadius, _targetMask);
        foreach (var target in targets)
        {
            var dirToTarget = (target.transform.position - _mainCamera.transform.position).normalized;
            float currentAngle = Vector3.Angle(_mainCamera.transform.forward, dirToTarget);
            if (currentAngle < _viewAngle * 0.5f)
            {
                if (currentAngle < shortestAngle)
                {
                    float distToTarget = Vector3.Distance(_mainCamera.transform.position, target.transform.position);
                    if (!Physics.Raycast(_mainCamera.transform.position, dirToTarget, distToTarget, _obstacleMask))
                    {
                        finalTarget = target.transform;
                        shortestAngle = currentAngle;
                    }
                }
            }
        }

        LockedTarget = finalTarget;
    }

    private void CanLockable()
    {
        if (!LockedTarget.gameObject.activeInHierarchy)
        {
            LockedTarget = null;
            return;
        }

        float distToTarget = Vector3.Distance(_mainCamera.transform.position, LockedTarget.position);
        if (distToTarget > _viewRadius)
        {
            LockedTarget = null;
            return;
        }

        var dirToTarget = (LockedTarget.position - _mainCamera.transform.position).normalized;
        if (Physics.Raycast(_mainCamera.transform.position, dirToTarget, distToTarget, _obstacleMask))
        {
            LockedTarget = null;
            return;
        }

        float pitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        if (_bottomClamp > pitch || pitch > _topClamp)
        {
            LockedTarget = null;
        }
    }

    private void FindTargetOrReset(InputAction.CallbackContext context)
    {
        if (IsLockOn)
        {
            LockedTarget = null;
        }
        else
        {
            FindLockableTarget();
        }
    }

    private void CalcTrackedObjectOffset()
    {
        var lookAtPos = (LockedTarget.position + transform.position) * 0.5f;
        var dist = Vector3.Distance(LockedTarget.position, transform.position);
        _targetComposer.m_TrackedObjectOffset.y =
            Mathf.Lerp(-lookAtPos.y, lookAtPos.y * 0.25f, (lookAtPos.magnitude - dist) * 0.25f);

        //_targetComposer.m_TrackedObjectOffset.y = dist < 3f ?
        //    lookAtPos.y * 0.25f : ClampAngle(-lookAtPos.y, _bottomClamp, _topClamp);

        //_targetComposer.m_TrackedObjectOffset.y = dist < lookAtPos.magnitude ? lookAtPos.y * 0.25f : -lookAtPos.y;
        //_targetComposer.m_TrackedObjectOffset.y =
        //    lerp ? Mathf.Lerp(-lookAtPos.y, lookAtPos.y * 0.25f, (lookAtPos.magnitude - dist) * 0.25f) : dist < lookAtPos.magnitude
        //    ? lookAtPos.y * 0.25f : -lookAtPos.y;
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle > 180)
        {
            return lfAngle - 360f;
        }

        if (lfAngle < -180)
        {
            return lfAngle + 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var vector3SaveData = saveData[0].ToObject<Vector3SaveData>();
        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(vector3SaveData.ToVector3());
    }

    protected override void OnDestroy()
    {
        if (Managers.GetInstance != null)
        {
            Managers.Input.GetAction("LockOn").performed -= FindTargetOrReset;
        }

        base.OnDestroy();
    }
}
