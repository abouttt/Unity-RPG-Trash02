using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class UI_ConfirmationPopup : UI_Popup
{
    enum Texts
    {
        ConfirmationText,
        OKText,
        NOText,
    }

    enum Buttons
    {
        OKButton,
        NOButton,
    }

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.NOButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ConfirmationPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    // �� ��ư�� �̺�Ʈ ����.
    public void SetEvent(UnityAction callback, string text, string OKText = "��", string NOText = "�ƴϿ�", bool hasOK = true, bool hasNO = true)
    {
        GetButton((int)Buttons.OKButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.OKButton).onClick.AddListener(callback);
        GetButton((int)Buttons.OKButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
        GetButton((int)Buttons.OKButton).gameObject.SetActive(hasOK);
        GetButton((int)Buttons.NOButton).gameObject.SetActive(hasNO);
        GetText((int)Texts.OKText).text = OKText;
        GetText((int)Texts.NOText).text = NOText;
        GetText((int)Texts.ConfirmationText).text = text;
    }
}
