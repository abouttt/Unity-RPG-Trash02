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

    [Serializable]
    public struct StatusSaveData
    {
        public int Level;
        public int CurrentHP;
        public int CurrentMP;
        public int CurrentXP;
        public int Gold;
        public int SkillPoint;
    }
}
