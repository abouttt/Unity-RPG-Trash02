using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Monster/Data", fileName = "Monster Data")]
public class MonsterData : ScriptableObject
{
    [field: SerializeField]
    public string MonsterID { get; private set; }

    [field: SerializeField]
    public string MonsterName { get; private set; }

    [field: SerializeField]
    public int MaxHP { get; private set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public int Defense { get; private set; }

    [field: SerializeField]
    public IntRange DropXP { get; private set; }

    [field: SerializeField]
    public IntRange DropGold { get; private set; }

    [field: SerializeField]
    public List<LootData> LootItems { get; private set; }

    public int GetXP() => Random.Range(DropXP.Min, DropXP.Max + 1);

    public int GetGold() => Random.Range(DropGold.Min, DropGold.Max + 1);

    public void DropItems(Vector3 position)
    {
        if (LootItems == null || LootItems.Count == 0)
        {
            return;
        }

        var go = Managers.Resource.Instantiate("FieldItem.prefab", null, true);
        var fieldItem = go.GetComponent<FieldItem>();

        foreach (var item in LootItems)
        {
            if (Random.Range(0, 101) <= item.DropProbability)
            {
                fieldItem.AddItem(item.ItemData, Random.Range(item.Count.Min, item.Count.Max + 1));
            }
        }

        go.transform.position = position;
    }
}
