using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : BaseMonoBehaviour
{
    public bool CanAttack { get; set; } = true;

    public bool IsAttacking { get; private set; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanAttack = value;
        }
    }

    [SerializeField]
    private float _attackRequiredSP;

    private bool _hasReservedAttack;
    private bool _enabled;

    // animation IDs
    private readonly int _animIDAttack = Animator.StringToHash("Attack");

    private void Awake()
    {
        Managers.Input.GetAction("Attack").performed += context => _hasReservedAttack = true;
    }

    private void Start()
    {
        Enabled = true;
    }

    private void Update()
    {
        if (_hasReservedAttack)
        {
            Attack();
        }
    }

    public void Clear()
    {
        IsAttacking = false;
        _hasReservedAttack = false;
    }

    private void Attack()
    {
        if (!CanAttack)
        {
            return;
        }

        _hasReservedAttack = false;

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        IsAttacking = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetBool(_animIDAttack, true);
    }

    private void OnBeginAttack()
    {
        Player.Status.SP -= _attackRequiredSP;
        Player.Animator.SetBool(_animIDAttack, false);
    }

    private void OnCanAttackCombo()
    {
        CanAttack = true;
        OnCanRoll();
    }

    private void OnCanRoll()
    {
        Player.Movement.CanRoll = true;
    }
}
