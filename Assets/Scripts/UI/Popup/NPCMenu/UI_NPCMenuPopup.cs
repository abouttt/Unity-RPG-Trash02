using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_NPCMenuPopup : UI_Popup
{
    enum RectTransforms
    {
        NPCMenuSubitems,
    }

    enum Texts
    {
        HeaderText
    }

    private NPC _npc;
    private readonly List<UI_NPCMenuSubitem> _subitems = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
    }

    private void Start()
    {
        Managers.UI.Register<UI_NPCMenuPopup>(this);

        Showed += () =>
        {
            Player.Movement.Enabled = false;
        };

        Closed += () =>
        {
            Clear();
            Player.Movement.Enabled = true;
        };
    }

    public void SetNPC(NPC npc)
    {
        if (npc == null)
        {
            return;
        }

        _npc = npc;

        GetText((int)Texts.HeaderText).text = npc.NPCName;

        foreach (var menu in npc.Menus)
        {
            AddSubitem(menu.MenuName, menu.Run);
        }

        AddSubitem("������", () =>
        {
            Managers.UI.Close<UI_NPCMenuPopup>();
        });
    }

    private void AddSubitem(string text, UnityAction callback)
    {
        var go = Managers.Resource.Instantiate("UI_NPCMenuSubitem.prefab", GetRT((int)RectTransforms.NPCMenuSubitems), true);
        var subitem = go.GetComponent<UI_NPCMenuSubitem>();
        subitem.SetEvent(text, callback);
        _subitems.Add(subitem);
    }

    private void Clear()
    {
        foreach (var subitem in _subitems)
        {
            subitem.Clear();
            Managers.Resource.Destroy(subitem.gameObject);
        }

        _npc.IsInteracted = false;
        _npc = null;
        _subitems.Clear();
    }
}
