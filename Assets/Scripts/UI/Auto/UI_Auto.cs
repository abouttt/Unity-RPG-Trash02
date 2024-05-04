using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    private void Start()
    {
        Managers.UI.Get<UI_AutoCanvas>().AddAutoUI(this);
    }
}
