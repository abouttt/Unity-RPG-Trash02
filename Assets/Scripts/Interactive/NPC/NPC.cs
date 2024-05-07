using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

    public IReadOnlyCollection<BaseNPCMenu> Menus => _menus;

    private BaseNPCMenu[] _menus;

    protected override void Awake()
    {
        base.Awake();

        _menus = GetComponents<BaseNPCMenu>();
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }
}
