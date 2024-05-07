using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

    public override void Interaction()
    {
        
    }
}
