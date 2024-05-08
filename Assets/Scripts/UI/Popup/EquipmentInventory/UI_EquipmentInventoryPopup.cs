using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

public class UI_EquipmentInventoryPopup : UI_Popup
{
    enum Texts
    {
        HPText,
        MPText,
        SPText,
        DamageText,
        DefenseText,
    }

    enum Buttons
    {
        CloseButton,
    }

    enum EquipmentSlots
    {
        UI_EquipmentSlot_Helmet,
        UI_EquipmentSlot_Chest,
        UI_EquipmentSlot_Pants,
        UI_EquipmentSlot_Boots,
        UI_EquipmentSlot_Weapon,
        UI_EquipmentSlot_Shield,
    }

    private readonly Dictionary<EquipmentType, UI_EquipmentSlot> _equipmentSlots = new();

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<UI_EquipmentSlot>(typeof(EquipmentSlots));

        Player.Status.HPChanged += RefreshHPText;
        Player.Status.MPChanged += RefreshMPText;
        Player.Status.SPChanged += RefreshSPText;
        Player.Status.StatChanged += RefreshAllStatusText;
        Player.EquipmentInventory.InventoryChanged += equipmentType => _equipmentSlots[equipmentType].Refresh();

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_EquipmentInventoryPopup>);

        InitSlots();
    }

    private void Start()
    {
        Managers.UI.Register<UI_EquipmentInventoryPopup>(this);

        foreach (var kvp in _equipmentSlots)
        {
            kvp.Value.Refresh();
        }

        RefreshAllStatusText();
    }

    private void RefreshAllStatusText()
    {
        RefreshHPText();
        RefreshMPText();
        RefreshSPText();
        RefreshDamageText();
        RefreshDefenseText();
    }

    private void RefreshHPText() => GetText((int)Texts.HPText).text = $"ü�� : {Player.Status.HP} / {Player.Status.MaxHP}";
    private void RefreshMPText() => GetText((int)Texts.MPText).text = $"���� : {Player.Status.MP} / {Player.Status.MaxMP}";
    private void RefreshSPText() => GetText((int)Texts.SPText).text = $"��� : {(int)Player.Status.SP} / {Player.Status.MaxSP}";
    private void RefreshDamageText() => GetText((int)Texts.DamageText).text = $"���ݷ� : {Player.Status.Damage}";
    private void RefreshDefenseText() => GetText((int)Texts.DefenseText).text = $"���� : {Player.Status.Defense}";

    private void InitSlots()
    {
        var equipmentTypes = Enum.GetValues(typeof(EquipmentType));
        var equipmentSlots = Enum.GetValues(typeof(EquipmentSlots));
        for (int i = 0; i < equipmentTypes.Length; i++)
        {
            EquipmentType type = (EquipmentType)equipmentTypes.GetValue(i);
            EquipmentSlots slot = (EquipmentSlots)equipmentSlots.GetValue(i);
            _equipmentSlots.Add(type, Get<UI_EquipmentSlot>((int)slot));
        }
    }
}
