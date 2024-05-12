using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Structs;

public class PlayerMovement : BaseMonoBehaviour, ISavable
{
    public static string SaveKey => "SaveTransform";

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanMove = value;
            CanRotation = value;
            CanSprint = value;
            CanJump = value;
            CanRoll = value;
        }
    }

    public bool CanMove { get; set; }
    public bool CanRotation { get; set; }
    public bool CanSprint { get; set; }
    public bool CanJump { get; set; }
    public bool CanRoll { get; set; }

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsRolling { get; private set; }

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _rollSpeed;

    [SerializeField]
    private float _rotationSmoothTime;

    [SerializeField]
    private float _speedChangeRate;

    [SerializeField]
    private float _jumpLandDecreaseSpeedRate;

    [Space(10)]
    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private float _jumpTimeout;

    [SerializeField]
    private float _fallTimeout;

    [Space(10)]
    [SerializeField]
    private float _groundedOffset;

    [SerializeField]
    private float _groundedRadius;

    [SerializeField]
    private LayerMask _groundLayers;

    [Space(10)]
    [SerializeField]
    private float _sprintRequiredSP;

    [SerializeField]
    private float _jumpRequiredSP;

    [SerializeField]
    private float _rollRequiredSP;

    private float _speed;
    private float _animationBlend;
    private float _posXBlend;
    private float _posYBlend;
    private float _targetMove;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private bool _isJumpLand;
    private bool _isJumpWithSprint;

    private bool _isRollMoving;

    private bool _enabled;

    // animation IDs
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosY = Animator.StringToHash("PosY");
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");
    private readonly int _animIDRoll = Animator.StringToHash("Roll");

    private GameObject _mainCamera;
    private CharacterController _controller;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _controller = GetComponent<CharacterController>();

        Managers.Input.GetAction("Jump").performed += Jump;
        Managers.Input.GetAction("Roll").performed += Roll;
    }

    private void Start()
    {
        Enabled = true;
        _targetRotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        _targetMove = _targetRotation;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        Gravity(deltaTime);
        CheckGrounded();
        Move(deltaTime);
    }

    public void ClearJump()
    {
        IsJumping = false;
        _isJumpLand = false;
        _isJumpWithSprint = false;
    }

    public void ClearRoll()
    {
        IsRolling = false;
        _isRollMoving = false;
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();
        var vector3SaveData = new Vector3SaveData(transform.position);
        saveData.Add(JObject.FromObject(vector3SaveData));
        saveData.Add(transform.rotation.eulerAngles.y);
        return saveData;
    }

    private void Gravity(float deltaTime)
    {
        if (IsGrounded)
        {
            // �߶� ���ѽð� ����
            _fallTimeoutDelta = _fallTimeout;

            Player.Animator.SetBool(_animIDFreeFall, false);

            // �������� �� �ӵ��� ������ �������� ���� ����
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            // ���� ���ѽð�
            if (_jumpTimeoutDelta >= 0f)
            {
                _jumpTimeoutDelta -= deltaTime;
            }
        }
        else
        {
            // ���� ���ѽð� ����
            _jumpTimeoutDelta = _jumpTimeout;

            // �߶� ���ѽð�
            if (_fallTimeoutDelta >= 0f)
            {
                _fallTimeoutDelta -= deltaTime;
            }
            else
            {
                Player.Animator.SetBool(_animIDFreeFall, true);
            }
        }

        // �͹̳� �Ʒ��� �ִ� ��� �ð��� ���� �߷��� ����
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * deltaTime;
        }
    }

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
        Player.Animator.SetBool(_animIDGrounded, IsGrounded);
    }

    private void Move(float deltaTime)
    {
        float targetSpeed = _moveSpeed;
        float requiredSP = 0f;

        // ���ַ� ���� ����� �� �Һ��� �� ���ְ� �Ұ��� �� �� Ű�� ���� �ٽ� �����ϵ���.
        if (!Managers.Input.Sprint && !CanSprint)
        {
            CanSprint = true;
        }

        if (_isRollMoving)
        {
            targetSpeed = _rollSpeed;
        }
        else if (_isJumpLand)
        {
            targetSpeed *= _jumpLandDecreaseSpeedRate;
        }
        else if (IsJumping && _isJumpWithSprint)
        {
            targetSpeed = _sprintSpeed;
        }

        if (Managers.Input.Sprint && CanSprint && !IsJumping! && !IsRolling)
        {
            if (Player.Status.SP > 0f)
            {
                IsSprinting = true;
                targetSpeed = _sprintSpeed;
                requiredSP = _sprintRequiredSP * deltaTime;
            }
            else
            {
                CanSprint = false;
            }
        }
        else
        {
            IsSprinting = false;
        }

        var move = Managers.Input.Move;
        bool isZeroMoveInput = move == Vector2.zero;

        if (!_isRollMoving && (!CanMove || isZeroMoveInput))
        {
            targetSpeed = 0f;
            requiredSP = 0f;
        }

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float currentSpeedChangeRate = deltaTime * _speedChangeRate;
        float speedOffset = 0.1f;

        // ��ǥ �ӵ����� ������
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, currentSpeedChangeRate);

            // �ӵ��� �Ҽ��� ���� 3�ڸ����� �ݿø�
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, currentSpeedChangeRate);
        _posXBlend = Mathf.Lerp(_posXBlend, move.x, currentSpeedChangeRate);
        _posYBlend = Mathf.Lerp(_posYBlend, move.y, currentSpeedChangeRate);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
            _posXBlend = 0f;
            _posYBlend = 0f;
        }

        if (CanRotation && !isZeroMoveInput)
        {
            var inputDirection = new Vector3(move.x, 0f, move.y).normalized;
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            _targetMove = _targetRotation;
        }

        bool isLockOn = Player.Camera.IsLockOn;
        bool isOnlyRun = !IsSprinting && !IsJumping && !IsRolling;

        if (isLockOn)
        {
            // ����, ����, ������� �� �½ÿ� ��ǲ �������� ȸ���Ѵ� �ƴϸ� Ÿ�� �������� ���ϵ��� ȸ��.
            if (isOnlyRun)
            {
                if (!isZeroMoveInput)
                {
                    var dirToTarget = (Player.Camera.LockedTarget.position - transform.position).normalized;
                    _targetRotation = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
                }
            }
            else
            {
                // �̵�Ű�� �ȴ����� ĳ���Ͱ� �ٶ󺸰� �ִ� �������� ȸ���� ����
                if (isZeroMoveInput)
                {
                    _targetMove = _targetRotation;
                }
                else
                {
                    _targetRotation = _targetMove;
                }
            }
        }

        // ȸ��
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);

        // �̵�
        var targetDirection = Quaternion.Euler(0f, _targetMove, 0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * deltaTime) + new Vector3(0f, _verticalVelocity, 0f) * deltaTime);
        Player.Status.SP -= requiredSP;

        // �ִϸ����� ������Ʈ
        bool isLockMoving = isLockOn && isOnlyRun;
        Player.Animator.SetFloat(_animIDSpeed, _animationBlend);
        Player.Animator.SetFloat(_animIDPosX, isLockMoving ? _posXBlend : 0f);
        Player.Animator.SetFloat(_animIDPosY, isLockMoving ? _posYBlend : 1f);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (CanJump && _jumpTimeoutDelta <= 0f && Player.Status.SP > 0f)
        {
            IsJumping = true;
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _isJumpWithSprint = Managers.Input.Sprint;
            Player.Animator.SetBool(_animIDJump, true);
        }
    }

    private void Roll(InputAction.CallbackContext context)
    {
        if (CanRoll && Player.Status.SP > 0f)
        {
            IsRolling = true;
            CanRotation = true;
            Player.Animator.SetBool(_animIDRoll, true);
        }
    }

    private void OnBeginJump()
    {
        Player.Status.SP -= _jumpRequiredSP;
        Player.Animator.SetBool(_animIDJump, false);
    }

    private void OnBeginJumpLand()
    {
        _isJumpLand = true;
    }

    private void OnBeginRoll()
    {
        _isRollMoving = true;
        Player.Status.SP -= _rollRequiredSP;
        Player.Animator.SetBool(_animIDRoll, false);
    }

    private void OnEndRoll()
    {
        IsRolling = false;
        CanRotation = true;
        CanSprint = true;
    }

    private void OnDrawGizmosSelected()
    {
        // IsGrounded �Ǵ� �ð�ȭ
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, _groundedRadius);
    }
}
