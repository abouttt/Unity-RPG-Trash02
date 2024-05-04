using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Cooldownable Database", fileName = "CooldownableDatabase")]
public class CooldownableDatabase : SingletonScriptableObject<CooldownableDatabase>
{
    public IReadOnlyList<ItemData> CooldownableItems => _cooldownableItems;

    [SerializeField]
    private List<ItemData> _cooldownableItems;

#if UNITY_EDITOR
    [ContextMenu("Find Cooldown")]
    public void FindCooldownable()
    {
        FindCooldownable<ICooldownable>();
    }

    private void FindCooldownable<T>() where T : ICooldownable
    {
        _cooldownableItems = new();
        foreach (var itemData in ItemDatabase.GetInstance.Items)
        {
            if (itemData is ICooldownable)
            {
                _cooldownableItems.Add(itemData);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
