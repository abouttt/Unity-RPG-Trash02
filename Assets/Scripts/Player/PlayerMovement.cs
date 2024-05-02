using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
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
        }
    }

    public bool CanMove { get; set; }
    public bool CanRotation { get; set; }
    public bool CanSprint { get; set; }
    public bool CanJump { get; set; }

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }

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

    private float _speed;
    private float _animationBlend;
    private float _posXBlend;
    private float _posYBlend;
    private float _targetRotation;
    private float _targetMove;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private bool _isJumpLand;
    private bool _isJumpWithSprint;

    private bool _enabled;

    // input
    private Vector2 _move;
    private bool _sprint;
    private bool _jump;

    // animation IDs
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosY = Animator.StringToHash("PosY");
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");

    private CharacterController _controller;
    private GameObject _mainCamera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main.gameObject;
    }

    private void Start()
    {
        Enabled = true;
        _targetRotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        JumpAndGravity(deltaTime);
        CheckGrounded();
        Move(deltaTime);
    }

    public void ClearJump()
    {
        IsJumping = false;
        _isJumpLand = false;
        _isJumpWithSprint = false;
    }

    private void JumpAndGravity(float deltaTime)
    {
        if (IsGrounded)
        {
            // �߶� ���ѽð� ����
            _fallTimeoutDelta = _fallTimeout;

            Player.Animator.SetBool(_animIDJump, false);
            Player.Animator.SetBool(_animIDFreeFall, false);

            // �������� �� �ӵ��� ������ �������� ���� ����
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jump && CanJump && _jumpTimeoutDelta <= 0f)
            {
                IsJumping = true;
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _isJumpWithSprint = _sprint;
                Player.Animator.SetBool(_animIDJump, true);
            }
            else
            {
                _jump = false;
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

        if (_isJumpLand)
        {
            targetSpeed *= _jumpLandDecreaseSpeedRate;
        }
        else if (IsJumping && _isJumpWithSprint)
        {
            targetSpeed = _sprintSpeed;
        }
        else if (_sprint && CanSprint && !IsJumping)
        {
            IsSprinting = true;
            targetSpeed = _sprintSpeed;
        }
        else
        {
            IsSprinting = false;
        }

        bool isZeroMoveInput = _move == Vector2.zero;

        if (!CanMove || isZeroMoveInput)
        {
            targetSpeed = 0f;
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
        _posXBlend = Mathf.Lerp(_posXBlend, _move.x, currentSpeedChangeRate);
        _posYBlend = Mathf.Lerp(_posYBlend, _move.y, currentSpeedChangeRate);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = _posXBlend = _posYBlend = 0f;
        }

        if (CanRotation && !isZeroMoveInput)
        {
            var inputDirection = new Vector3(_move.x, 0f, _move.y).normalized;
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            _targetMove = _targetRotation;
        }

        bool isLockOn = Player.Camera.IsLockOn;
        bool isOnlyRun = !IsSprinting && !IsJumping;

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

        // �ִϸ����� ������Ʈ
        bool isLockMoving = isLockOn && isOnlyRun;
        Player.Animator.SetFloat(_animIDSpeed, _animationBlend);
        Player.Animator.SetFloat(_animIDPosX, isLockMoving ? _posXBlend : 0f);
        Player.Animator.SetFloat(_animIDPosY, isLockMoving ? _posYBlend : 1f);
    }

    private void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    private void OnSprint(InputValue inputValue)
    {
        _sprint = inputValue.isPressed;
    }

    private void OnJump(InputValue inputValue)
    {
        _jump = inputValue.isPressed;
    }

    private void OnAnimJumpLand()
    {
        _isJumpLand = true;
    }

    private void OnDrawGizmosSelected()
    {
        // IsGrounded �Ǵ� �ð�ȭ
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, _groundedRadius);
    }
}
