using System;

namespace Structs
{
    [Serializable]
    public struct ItemSaveData
    {
        public string ItemID;
        public int Count;
        public int Index;
    }

    [Serializable]
    public struct SkillSaveData
    {
        public string SkillID;
        public int CurrentLevel;
    }

    [Serializable]
    public struct QuickSaveData
    {
        public ItemSaveData? ItemSaveData;
        public SkillSaveData? SkillSaveData;
        public int Index;
    }
}
