using UnityEngine;
using EnumType;

public class UI_MainMenuFixed : UI_Base
{
    enum Buttons
    {
        ContinueButton,
        NewGameButton,
        OptionButton,
        BackButton,
        ExitButton,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_MainMenuFixed>(this);

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {

        });

        GetButton((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            Managers.Game.IsDefaultSpawn = true;
            Managers.Scene.LoadScene(SceneType.VillageScene);
        });

        GetButton((int)Buttons.OptionButton).onClick.AddListener(() => EnabledOptionMenu(true));

        GetButton((int)Buttons.BackButton).onClick.AddListener(() => EnabledOptionMenu(false));

        GetButton((int)Buttons.ExitButton).onClick.AddListener(() =>
        {
            Managers.Data.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    private void Start()
    {
        EnabledOptionMenu(false);
    }

    private void EnabledOptionMenu(bool enabled)
    {
        if (enabled)
        {
            Managers.UI.Show<UI_GameOptionPopup>();
        }
        else
        {
            Managers.UI.Close<UI_GameOptionPopup>();
        }

        GetButton((int)Buttons.ContinueButton).gameObject.SetActive(false);
        GetButton((int)Buttons.NewGameButton).gameObject.SetActive(!enabled);
        GetButton((int)Buttons.OptionButton).gameObject.SetActive(!enabled);
        GetButton((int)Buttons.BackButton).gameObject.SetActive(enabled);
        GetButton((int)Buttons.ExitButton).gameObject.SetActive(!enabled);
    }
}
