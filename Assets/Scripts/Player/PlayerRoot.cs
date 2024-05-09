using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using EnumType;

public class PlayerRoot : BaseMonoBehaviour
{
    [Serializable]
    public class EquipData
    {
        public Transform Root;
        [ReadOnly]
        public GameObject Equipment;
    }

    [SerializeField]
    private SerializedDictionary<EquipmentType, EquipData> _equipDatas;

    [SerializeField]
    private RuntimeAnimatorController _basicAnimator;

    [SerializeField]
    private AnimatorOverrideController _combatAnimator;

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += RefreshEquipmentObject;
    }

    private void Start()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType type in types)
        {
            RefreshEquipmentObject(type);
        }
    }

    private void RefreshEquipmentObject(EquipmentType equipmentType)
    {
        if (!_equipDatas.TryGetValue(equipmentType, out _))
        {
            return;
        }

        var equipData = _equipDatas[equipmentType];

        if (Player.EquipmentInventory.IsEquipped(equipmentType))
        {
            if (equipData.Equipment != null)
            {
                Destroy(equipData.Equipment);
            }

            var equipment = Instantiate(Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.EquipmentPrefab, equipData.Root);
            equipData.Equipment = equipment;
        }
        else
        {
            Destroy(equipData.Equipment);
            equipData.Equipment = null;
        }

        RefreshAnimator();
    }

    private void RefreshAnimator()
    {
        bool hasEquipment = false;

        if (Player.EquipmentInventory.IsEquipped(EquipmentType.Weapon) ||
            Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            hasEquipment = true;
        }

        Player.Animator.runtimeAnimatorController = hasEquipment ? _combatAnimator : _basicAnimator;
    }
}
