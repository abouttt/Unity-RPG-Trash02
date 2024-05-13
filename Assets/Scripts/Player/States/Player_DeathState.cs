using UnityEngine;
using EnumType;

public class Player_DeathState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Movement.Enabled = false;
        Player.Combat.Enabled = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            if (Managers.UI.IsShowed<UI_ConfirmationPopup>())
            {
                return;
            }

            Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
            {
                Player.Status.HP = Player.Status.MaxHP;
                Player.Status.MP = Player.Status.MaxMP;
                Managers.Game.IsDefaultSpawn = true;
                Managers.Scene.LoadScene(SceneType.VillageScene);
            }, "Ȯ���� �����ø� �������� ��Ȱ�ϰ� �˴ϴ�.", "Ȯ��", null, true, false);
        }
    }
}
