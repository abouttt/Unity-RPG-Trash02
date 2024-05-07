using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCConversation : BaseNPCMenu
{
    public NPC Owner { get; private set; }
    public IReadOnlyList<string> ConversationScripts => _conversationScripts;

    [field: SerializeField, TextArea, Space(10)]
    private List<string> _conversationScripts;

    private void Awake()
    {
        Owner = GetComponent<NPC>();
        MenuName = "¥Î»≠";
    }

    public override void Execution()
    {
        Managers.UI.Show<UI_ConversationPopup>().SetNPCConversation(this);
    }
}
