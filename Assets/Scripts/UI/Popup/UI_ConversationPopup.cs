using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_ConversationPopup : UI_Popup, IPointerClickHandler
{
    enum Texts
    {
        NPCNameText,
        ScriptText,
    }

    enum Buttons
    {
        CloseButton,
    }

    [SerializeField]
    private float _typingSpeed;

    private NPCConversation _npcConversation;
    private int _currentIndex = 0;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ConversationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ConversationPopup>(this);

        Showed += () =>
        {
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(false);
        };

        Closed += () =>
        {
            _npcConversation = null;
            GetText((int)Texts.ScriptText).DOKill();
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPCConversation(NPCConversation npc)
    {
        _npcConversation = npc;
        _currentIndex = 0;
        GetText((int)Texts.NPCNameText).text = npc.Owner.NPCName;
        GetText((int)Texts.ScriptText).text = null;
        GetText((int)Texts.ScriptText).DOText(
            npc.ConversationScripts[_currentIndex], _npcConversation.ConversationScripts[_currentIndex].Length / _typingSpeed);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var script = GetText((int)Texts.ScriptText);
        if (script.text.Length == _npcConversation.ConversationScripts[_currentIndex].Length)
        {
            _currentIndex++;
            if (_currentIndex >= _npcConversation.ConversationScripts.Count)
            {
                Managers.UI.Close<UI_ConversationPopup>();
                return;
            }

            script.text = null;
            script.DOText(_npcConversation.ConversationScripts[_currentIndex], _npcConversation.ConversationScripts[_currentIndex].Length / _typingSpeed);
        }
        else
        {
            script.DOKill();
            script.text = _npcConversation.ConversationScripts[_currentIndex];
        }
    }
}
