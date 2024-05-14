using UnityEngine;
using EnumType;

public class SkillEvents : MonoBehaviour
{
    [SerializeField]
    private ActiveSkillData _shieldAttackData;

    [SerializeField]
    private ActiveSkillData _chargeAttackSkillData;

    private GameObject _charge;

    private void Start()
    {
        Player.Combat.Damaged += () =>
        {
            if (_charge != null)
            {
                Managers.Resource.Destroy(_charge);
            }
        };
    }

    public void OnBeginShieldAttack()
    {
        (Player.SkillTree.GetSkillByData(_shieldAttackData) as ActiveSkill).SubRequired();
    }

    public void OnShieldAttack()
    {
        var shield = Player.Root.GetEquipment(EquipmentType.Shield);
        Managers.Resource.Instantiate(
            "ShieldAttack.prefab", shield.transform.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f), null, true);
        var targets = Physics.OverlapSphere(shield.transform.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var target in targets)
        {
            var monster = target.GetComponent<Monster>();
            var skill = Player.SkillTree.GetSkillByData(_shieldAttackData);
            monster.Stunned(Player.Status.Damage + _shieldAttackData.FixedStats[skill.CurrentLevel - 1].Damage);
        }
    }

    public void OnBeginChargeAttack()
    {
        (Player.SkillTree.GetSkillByData(_chargeAttackSkillData) as ActiveSkill).SubRequired();
    }

    public void OnChargeAttack_Charge()
    {
        var weapon = Player.Combat.Weapon;
        _charge = Managers.Resource.Instantiate("ChargeAttack_Charge.prefab", weapon.Middle.position, weapon.transform, true);
    }

    public void OnChargeAttack_Attack()
    {
        Managers.Resource.Destroy(_charge);

        var weapon = Player.Combat.Weapon;
        Managers.Resource.Instantiate("ChargeAttack_Attack.prefab", weapon.Top.position, null, true);
        var targets = Physics.OverlapSphere(weapon.Top.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var target in targets)
        {
            var monster = target.GetComponent<Monster>();
            var skill = Player.SkillTree.GetSkillByData(_chargeAttackSkillData);
            monster.TakeDamage(Player.Status.Damage + _chargeAttackSkillData.FixedStats[skill.CurrentLevel - 1].Damage);
        }
    }
}
