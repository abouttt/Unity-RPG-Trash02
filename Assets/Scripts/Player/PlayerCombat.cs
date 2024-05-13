using UnityEngine;
using UnityEngine.InputSystem;
using EnumType;

public class PlayerCombat : BaseMonoBehaviour
{
    public bool CanAttack { get; set; }
    public bool CanParry { get; set; }
    public bool CanDefense { get; set; }

    public bool IsAttacking { get; private set; }
    public bool IsParrying { get; private set; }
    public bool IsDefending { get; private set; }
    public bool IsDamaging { get; private set; }
    public bool IsDefenseDamaging { get; private set; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanAttack = value;
            CanParry = value;
            CanDefense = value;
        }
    }

    [SerializeField]
    private float _defenseAngle;

    [SerializeField]
    private float _attackRequiredSP;

    [SerializeField]
    private float _parryRequiredSP;

    [SerializeField]
    private float _defenseDamagedRequiredSP;

    private Weapon _weapon;
    private bool _hasReservedAttack;
    private bool _isParryable;
    private bool _hasShield;
    private bool _enabled;

    // animation IDs
    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDParry = Animator.StringToHash("Parry");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");
    private readonly int _animIDDamaged = Animator.StringToHash("Damaged");

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += Refresh;
    }

    private void Start()
    {
        Enabled = true;
        Refresh(EquipmentType.Weapon);
        Refresh(EquipmentType.Shield);
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

    public void TakeDamage(Monster monster, Vector3 attackedPosition, int damage, bool parryable)
    {
        if (Player.Status.HP <= 0)
        {
            return;
        }

        if (Player.Movement.IsRolling)
        {
            return;
        }

        if (parryable && _isParryable)
        {
            if (IsInRangeOfDefenseAngle(attackedPosition))
            {
                monster.Stunned();
                Managers.Resource.Instantiate(
                    "SteelHit.prefab", Player.Root.GetEquipment(EquipmentType.Shield).transform.position, null, true);
                return;
            }
        }
        else if (IsDefending)
        {
            if (IsInRangeOfDefenseAngle(attackedPosition) && Player.Status.SP > 0)
            {
                HitShield();
                return;
            }
            else
            {
                OffDefense();
                CanDefense = false;
            }
        }

        IsDamaging = true;
        Player.Status.HP -= Util.CalcDamage(damage, Player.Status.Defense);

        if (Player.Status.HP <= 0)
        {
            Player.Animator.Play("Death", -1, 0f);
        }
        else
        {
            Player.Animator.Play("Damaged", -1, 0f);
            Player.Animator.SetBool(_animIDDamaged, true);
        }
    }

    public void HitShield(Vector3? hitPosition = null)
    {
        if (!IsDefending)
        {
            return;
        }

        IsDefenseDamaging = true;
        Player.Animator.Play("DefenseDamaged", -1, 0f);
        Managers.Resource.Instantiate("SteelHit.prefab",
            hitPosition != null ? hitPosition.Value : Player.Root.GetEquipment(EquipmentType.Shield).transform.position, null, true);
    }

    public void Clear()
    {
        IsAttacking = false;
        IsParrying = false;
        IsDefending = false;
        IsDefenseDamaging = false;
        IsDamaging = false;
        _hasReservedAttack = false;
        _isParryable = false;
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

    private void Parry(InputAction.CallbackContext context)
    {
        if (!CanParry)
        {
            return;
        }

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        IsParrying = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetBool(_animIDParry, true);
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

    private bool IsInRangeOfDefenseAngle(Vector3 attackedPosition)
    {
        var dir = (attackedPosition - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < _defenseAngle * 0.5f)
        {
            return true;
        }

        return false;
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
        OnCanParryAndRoll();
    }

    private void OnEnableWeapon()
    {
        _weapon.Enabled = true;
    }

    private void OnDisableWeapon()
    {
        _weapon.Enabled = false;
    }

    private void OnBeginParry()
    {
        Player.Status.SP -= _parryRequiredSP;
        Player.Animator.SetBool(_animIDParry, false);
    }

    private void OnEnableParry()
    {
        _isParryable = true;
    }

    private void OnDisableParry()
    {
        _isParryable = false;
    }

    private void OnCanParryAndRoll()
    {
        CanParry = true;
        Player.Movement.CanRoll = true;
    }

    private void OnBeginDefenseDamaged()
    {
        Player.Status.SP -= _defenseDamagedRequiredSP;
    }

    private void OnBeginDamaged()
    {
        Player.Animator.SetBool(_animIDDamaged, false);
    }

    private void Refresh(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                if (Player.EquipmentInventory.IsEquipped(equipmentType))
                {
                    Managers.Input.GetAction("Attack").performed += ReserveAttack;
                    _weapon = Player.Root.GetEquipment(EquipmentType.Weapon).GetComponent<Weapon>();
                }
                else
                {
                    Managers.Input.GetAction("Attack").performed -= ReserveAttack;
                    _weapon = null;
                }
                break;
            case EquipmentType.Shield:
                _hasShield = Player.EquipmentInventory.IsEquipped(equipmentType);
                if (_hasShield)
                {
                    Managers.Input.GetAction("Parry").performed += Parry;
                }
                else
                {
                    Managers.Input.GetAction("Parry").performed -= Parry;
                }
                break;
            default:
                break;
        }
    }

    protected override void OnDestroy()
    {
        if (Managers.GetInstance != null)
        {
            Managers.Input.GetAction("Attack").performed -= ReserveAttack;
            Managers.Input.GetAction("Parry").performed -= Parry;
        }

        base.OnDestroy();
    }
}
