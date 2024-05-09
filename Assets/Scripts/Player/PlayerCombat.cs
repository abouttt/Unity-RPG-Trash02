using UnityEngine;
using UnityEngine.InputSystem;
using EnumType;

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
        Player.EquipmentInventory.InventoryChanged += equipmentType =>
        {
            if (equipmentType == EquipmentType.Weapon)
            {
                if (Player.EquipmentInventory.IsEquipped(equipmentType))
                {
                    Managers.Input.GetAction("Attack").performed += ReserveAttack;
                }
                else
                {
                    Managers.Input.GetAction("Attack").performed -= ReserveAttack;
                }
            }
        };
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

    private void ReserveAttack(InputAction.CallbackContext context)
    {
        if (!Managers.Input.CursorLocked || Player.Movement.IsJumping || Player.Movement.IsRolling)
        {
            return;
        }

        _hasReservedAttack = true;
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
