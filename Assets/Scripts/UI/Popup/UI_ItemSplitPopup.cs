using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class UI_ItemSplitPopup : UI_Popup
{
    enum GameObjects
    {
        ItemPrice,
    }

    enum Texts
    {
        GuideText,
        PriceText,
    }

    enum Buttons
    {
        UpButton,
        DownButton,
        OKButton,
        NOButton,
    }

    enum InputFields
    {
        InputField,
    }

    public int CurrentCount { get; private set; }

    private int _price;
    private int _minCount;
    private int _maxCount;

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        Get<TMP_InputField>((int)InputFields.InputField).onValueChanged.AddListener(value => OnValueChanged(value));
        Get<TMP_InputField>((int)InputFields.InputField).onEndEdit.AddListener(value => OnEndEdit(value));

        GetButton((int)Buttons.UpButton).onClick.AddListener(() => OnClickUpOrDownButton(1));
        GetButton((int)Buttons.DownButton).onClick.AddListener(() => OnClickUpOrDownButton(-1));
        GetButton((int)Buttons.NOButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemSplitPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    public void SetEvent(UnityAction callback, string text, int minCount, int maxCount, int price = 0, bool showPrice = false)
    {
        GetButton((int)Buttons.OKButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.OKButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);
        GetButton((int)Buttons.OKButton).onClick.AddListener(callback);

        CurrentCount = _maxCount = maxCount;
        _minCount = minCount;
        _price = price;

        GetText((int)Texts.GuideText).text = text;
        Get<TMP_InputField>((int)InputFields.InputField).text = CurrentCount.ToString();
        GetObject((int)GameObjects.ItemPrice).SetActive(showPrice);
    }

    private void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            CurrentCount = 0;
        }
        else
        {
            CurrentCount = Mathf.Clamp(int.Parse(value), _minCount, _maxCount);
            Get<TMP_InputField>((int)InputFields.InputField).text = CurrentCount.ToString();
        }

        if (GetText((int)Texts.PriceText).gameObject.activeSelf)
        {
            int totalPrice = _price * CurrentCount;
            GetText((int)Texts.PriceText).text = totalPrice.ToString();
            GetText((int)Texts.PriceText).color = (_price * CurrentCount) <= Player.Status.Gold ? Color.white : Color.red;
        }
    }

    private void OnEndEdit(string value)
    {
        CurrentCount = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxCount : int.Parse(value), _minCount, _maxCount);
        var inputField = Get<TMP_InputField>((int)InputFields.InputField);
        inputField.text = CurrentCount.ToString();
        inputField.caretPosition = inputField.text.Length;
    }

    private void OnClickUpOrDownButton(int count)
    {
        CurrentCount = Mathf.Clamp(CurrentCount + count, _minCount, _maxCount);
        Get<TMP_InputField>((int)InputFields.InputField).text = CurrentCount.ToString();
    }
}
