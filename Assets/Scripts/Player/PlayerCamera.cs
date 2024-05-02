using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
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
                CalcTrackedObjectOffset(false);
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

    private Vector2 _look;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private readonly int _animIDLockOn = Animator.StringToHash("LockOn");
    private readonly float _threshold = 0.01f;

    private Animator _stateDrivenCameraAnimator;
    private CinemachineComposer _targetComposer;
    private UI_LockOnTargetImage _lockOnTargetImage;
    private GameObject _mainCamera;

    private void Awake()
    {
        _stateDrivenCameraAnimator = _stateDrivenCamera.GetComponent<Animator>();
        _mainCamera = Camera.main.gameObject;
        foreach (CinemachineVirtualCamera child in _stateDrivenCamera.ChildCameras)
        {
            _targetComposer = child.GetCinemachineComponent<CinemachineComposer>();
            if (_targetComposer != null)
            {
                break;
            }
        }
    }

    private void Start()
    {
        var go = Managers.Resource.Instantiate("UI_LockOnTargetImage.prefab");
        _lockOnTargetImage = go.GetComponent<UI_LockOnTargetImage>();

        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (IsLockOn)
        {
            CalcTrackedObjectOffset(true);
            CanLockable();
        }
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
            if (_look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += _look.x * _sensitivity;
                _cinemachineTargetPitch += _look.y * _sensitivity;
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
                float distToTarget = Vector3.Distance(_mainCamera.transform.position, target.transform.position);
                if (currentAngle < shortestAngle)
                {
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

    private void CalcTrackedObjectOffset(bool lerp)
    {
        var lookAtPos = (LockedTarget.position + transform.position) * 0.5f;
        var dist = Vector3.Distance(LockedTarget.position, transform.position);
        _targetComposer.m_TrackedObjectOffset.y =
            lerp ? Mathf.Lerp(-lookAtPos.y, 0f, (lookAtPos.magnitude - dist) * 0.15f) : dist < lookAtPos.magnitude ? 0f : -lookAtPos.y;
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

    private void OnLook(InputValue inputValue)
    {
        _look = inputValue.Get<Vector2>();
    }

    private void OnLockOnTarget(InputValue inputValue)
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
}
