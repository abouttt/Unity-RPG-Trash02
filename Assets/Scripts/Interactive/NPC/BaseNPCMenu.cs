using UnityEngine;

public abstract class BaseNPCMenu : BaseMonoBehaviour
{
    public string MenuName { get; protected set; }

    public abstract void Execution();
}
