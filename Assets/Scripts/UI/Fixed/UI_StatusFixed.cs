using UnityEngine;

public class UI_StatusFixed : UI_Base
{
    enum Images
    {
        HPBar,
        MPBar,
        SPBar,
        XPBar,
    }

    enum Texts
    {
        LevelText,
    }

    protected override void Init()
    {
        Managers.UI.Register<UI_StatusFixed>(this);

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        Player.Status.LevelChanged += RefreshLevelText;
        Player.Status.HPChanged += RefreshHPImage;
        Player.Status.MPChanged += RefreshMPImage;
        Player.Status.SPChanged += RefreshSPImage;
        Player.Status.XPChanged += RefreshXPImage;
        Player.Status.StatChanged += RefreshAll;
    }

    private void Start()
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        RefreshLevelText();
        RefreshHPImage();
        RefreshMPImage();
        RefreshSPImage();
        RefreshXPImage();
    }

    private void RefreshLevelText() => GetText((int)Texts.LevelText).text = $"Lev.{Player.Status.Level}";
    private void RefreshHPImage() => GetImage((int)Images.HPBar).fillAmount = (float)Player.Status.HP / Player.Status.MaxHP;
    private void RefreshMPImage() => GetImage((int)Images.MPBar).fillAmount = (float)Player.Status.MP / Player.Status.MaxMP;
    private void RefreshSPImage() => GetImage((int)Images.SPBar).fillAmount = (float)Player.Status.SP / Player.Status.MaxSP;
    private void RefreshXPImage() => GetImage((int)Images.XPBar).fillAmount = (float)Player.Status.XP / Player.Status.MaxXP;
}
