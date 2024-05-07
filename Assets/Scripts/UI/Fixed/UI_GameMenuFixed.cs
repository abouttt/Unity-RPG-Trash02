using UnityEngine;

public class UI_GameMenuFixed : UI_Base
{
    enum Buttons
    {
        GameMenuPopupButton,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_GameMenuFixed>(this);

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.GameMenuPopupButton).onClick.AddListener(Managers.UI.ShowOrClose<UI_GameMenuPopup>);
    }
}
