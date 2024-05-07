using UnityEngine;

public class UI_InteractionKeyGuide : UI_Auto
{
    enum Images
    {
        InputTimeImage,
        BG,
        Frame,
    }

    enum Texts
    {
        KeyText,
        InteractionText,
        NameText,
    }

    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();
    }

    private void Update()
    {
        if (GetImage((int)Images.InputTimeImage).gameObject.activeSelf)
        {
            GetImage((int)Images.InputTimeImage).fillAmount = Player.InteractionDetector.CurrentInteractionInputTime;
        }
    }

    public void SetTarget(Interactive target)
    {
        if (target != null)
        {
            _followTarget.SetTargetAndOffset(target.transform, target.InteractionKeyGuidePos);

            GetImage((int)Images.BG).gameObject.SetActive(target.CanInteraction);
            GetText((int)Texts.KeyText).gameObject.SetActive(target.CanInteraction);
            GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interaction");
            GetText((int)Texts.InteractionText).gameObject.SetActive(target.CanInteraction);
            GetText((int)Texts.InteractionText).text = target.InteractionMessage;

            if (target is NPC npc)
            {
                GetText((int)Texts.NameText).text = npc.NPCName;
                GetText((int)Texts.NameText).gameObject.SetActive(true);
            }
            else
            {
                GetText((int)Texts.NameText).gameObject.SetActive(false);
            }

            bool hasInputTime = target.InteractionInputTime > 0f;
            GetImage((int)Images.InputTimeImage).gameObject.SetActive(hasInputTime);
            GetImage((int)Images.Frame).gameObject.SetActive(hasInputTime);
        }

        gameObject.SetActive(target != null);
    }
}
