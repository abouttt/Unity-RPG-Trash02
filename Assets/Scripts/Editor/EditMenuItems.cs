#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static class EditorMenuItems
{
    [MenuItem("Tools/Player/Refresh Databases")]
    public static void RefreshDatabases()
    {
        ItemDatabase.GetInstance.FindItems();
        SkillDatabase.GetInstance.FindSkills();
        CooldownableDatabase.GetInstance.FindCooldownable();
        QuestDatabase.GetInstance.FindQuests();
    }

    [MenuItem("Tools/Player/Clear Save Data")]
    public static void ClearSaveData()
    {
        DirectoryInfo directory = new(DataManager.SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }
    }
}
#endif
