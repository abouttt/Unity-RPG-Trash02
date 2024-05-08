using System.Collections.Generic;
using UnityEngine;

public class NPCConversation : BaseNPCMenu
{
    public IReadOnlyList<string> ConversationScripts => _conversationScripts;

    [field: SerializeField, TextArea, Space(10)]
    private List<string> _conversationScripts;

    protected override void Awake()
    {
        base.Awake();

        MenuName = "��ȭ";
    }

    public override void Execution()
    {
        Managers.UI.Show<UI_ConversationPopup>().SetNPCConversation(this);
    }
}
