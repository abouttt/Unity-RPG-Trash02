using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

namespace Structs
{
    [Serializable]
    public struct Vector3SaveData
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3SaveData(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);
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
    public struct QuestSaveData
    {
        public string QuestID;
        public QuestState State;
        public Dictionary<string, int> Targets;
    }
}
