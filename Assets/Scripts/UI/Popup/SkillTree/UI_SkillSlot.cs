using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class UI_SkillSlot : UI_BaseSlot
{
    enum Imagess
    {
        SkillDisabledImage = 2,
        LevelUpDisabledImage,
    }

    enum Texts
    {
        LevelText,
    }

    enum Buttons
    {
        LevelUpButton,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    [field: SerializeField]
    public SkillData SkillData { get; private set; }

    private Skill SkillRef => ObjectRef as Skill;

    protected override void Init()
    {
        base.Init();

        CanDrag = SkillData.SkillType == SkillType.Active;

        BindImage(typeof(Imagess));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        SetObject(Player.SkillTree.GetSkillByData(SkillData), SkillData.SkillImage);

        GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
        GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
        GetButton((int)Buttons.LevelUpButton).onClick.AddListener(() =>
        {
            SkillRef.LevelUp();
            Managers.UI.Get<UI_SkillTreePopup>().SetTop();
        });


        if (SkillData is ICooldownable cooldownable)
        {
            Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
        }

        SkillRef.SkillChanged += Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (SkillRef.CurrentLevel == SkillData.MaxLevel)
        {
            GetImage((int)Imagess.SkillDisabledImage).gameObject.SetActive(false);
            GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(SkillRef.IsAcquirable);
            GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(Player.Status.SkillPoint < SkillData.RequiredSkillPoint);

            if (SkillRef.IsUnlocked)
            {
                GetImage((int)Imagess.SkillDisabledImage).gameObject.SetActive(false);
            }
            else if (!SkillRef.IsAcquirable)
            {
                CanDrag = false;
                GetImage((int)Imagess.SkillDisabledImage).gameObject.SetActive(true);
                GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
                GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
            }
        }

        RefreshLevelText();
    }

    private void RefreshLevelText()
    {
        GetText((int)Texts.LevelText).text = $"{SkillRef.CurrentLevel} / {SkillData.MaxLevel}";
    }

    private bool IsOnPointerSameGameObject(PointerEventData eventData, GameObject gameObject)
    {
        return eventData.pointerCurrentRaycast.gameObject == gameObject;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!SkillRef.IsUnlocked)
        {
            eventData.pointerDrag = null;
            return;
        }

        base.OnBeginDrag(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOnPointerSameGameObject(eventData, GetButton((int)Buttons.LevelUpButton).gameObject) ||
            IsOnPointerSameGameObject(eventData, GetImage((int)Imagess.LevelUpDisabledImage).gameObject))
        {
            return;
        }

        Managers.UI.Get<UI_TooltipTop>().SkillTooltip.SetSlot(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TooltipTop>().SkillTooltip.SetSlot(null);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_SkillTreePopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp() || !SkillRef.IsUnlocked)
        {
            return;
        }

        if (IsOnPointerSameGameObject(eventData, GetButton((int)Buttons.LevelUpButton).gameObject) ||
            IsOnPointerSameGameObject(eventData, GetImage((int)Imagess.LevelUpDisabledImage).gameObject))
        {
            return;
        }

        if (SkillRef is IUsable usable)
        {
            usable.Use();
        }
    }
}
