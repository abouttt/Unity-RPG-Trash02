using UnityEngine;
using UnityEngine.InputSystem;
using EnumType;

public class PlayerCombat : BaseMonoBehaviour
{
    public bool CanAttack { get; set; }
    public bool CanDefense { get; set; }

    public bool IsAttacking { get; private set; }
    public bool IsDefending { get; private set; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanAttack = value;
            CanDefense = value;
        }
    }

    [SerializeField]
    private float _attackRequiredSP;

    private bool _hasReservedAttack;
    private bool _hasShield;
    private bool _enabled;

    // animation IDs
    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += equipmentType =>
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    if (Player.EquipmentInventory.IsEquipped(equipmentType))
                    {
                        Managers.Input.GetAction("Attack").performed += ReserveAttack;
                    }
                    else
                    {
                        Managers.Input.GetAction("Attack").performed -= ReserveAttack;
                    }
                    break;
                case EquipmentType.Shield:
                    _hasShield = Player.EquipmentInventory.IsEquipped(equipmentType);
                    break;
                default:
                    break;
            }
        };
    }

    private void Start()
    {
        Enabled = true;
    }

    private void Update()
    {
        if (!Managers.Input.CursorLocked)
        {
            _hasReservedAttack = false;
            if (IsDefending)
            {
                OffDefense();
            }
            return;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Managers.Input.Defense && CanDefense && _hasShield)
        {
            Defense();
        }
        else if (IsDefending)
        {
            OffDefense();
        }
    }

    public void Clear()
    {
        IsAttacking = false;
        IsDefending = false;
        _hasReservedAttack = false;
        Player.Animator.SetBool(_animIDDefense, false);
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

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        IsAttacking = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetBool(_animIDAttack, true);
    }

    private void Defense()
    {
        IsDefending = true;
        Player.Animator.SetBool(_animIDDefense, true);
    }

    private void OffDefense()
    {
        IsDefending = false;
        Player.Animator.SetBool(_animIDDefense, false);
    }

    private void ReserveAttack(InputAction.CallbackContext context)
    {
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
