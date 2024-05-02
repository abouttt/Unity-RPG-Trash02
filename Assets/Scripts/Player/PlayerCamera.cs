using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("[Rotate]")]
    [SerializeField]
    private GameObject _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    private Vector2 _look;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private readonly float _threshold = 0.01f;

    private void Start()
    {
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
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
}
