using UnityEngine;

public class UI_LockOnTargetImage : UI_Auto
{
    public UI_FollowWorldObject FollowWorldObject { get; private set; }

    protected override void Init()
    {
        FollowWorldObject = GetComponent<UI_FollowWorldObject>();
    }
}
