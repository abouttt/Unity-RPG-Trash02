#if UNITY_EDITOR
using UnityEditor;

public static class EditorMenuItems
{
    [MenuItem("Tools/Player/Refresh Databases")]
    public static void RefreshDatabases()
    {
        ItemDatabase.GetInstance.FindItems();
        CooldownableDatabase.GetInstance.FindCooldownable();
        QuestDatabase.GetInstance.FindQuests();
    }
}
#endif
