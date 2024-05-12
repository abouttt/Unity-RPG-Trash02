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

}
