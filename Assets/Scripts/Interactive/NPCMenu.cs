using UnityEngine;

public abstract class NPCMenu : BaseMonoBehaviour
{
    public string MenuName {  get; private set; }

    public abstract void Run();
}
