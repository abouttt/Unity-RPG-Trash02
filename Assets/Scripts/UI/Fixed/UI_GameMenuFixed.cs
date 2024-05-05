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

        GetButton((int)Buttons.GameMenuPopupButton).onClick.AddListener(() =>
        {
            if (Managers.UI.IsShowed<UI_GameMenuPopup>())
            {
                Managers.UI.Close<UI_GameMenuPopup>();
            }
            else
            {
                Managers.UI.Show<UI_GameMenuPopup>();
            }
        });
    }
}
