using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

    public IReadOnlyCollection<NPCMenu> Menus => _menus;

    private NPCMenu[] _menus;

    protected override void Awake()
    {
        base.Awake();

        _menus = GetComponents<NPCMenu>();
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }
}
