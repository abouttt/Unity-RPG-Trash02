using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(IntRange))]
public class IntRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        {
            float x, w;
            int elementCount = 3;
            for (int i = 0; i < elementCount; i++)
            {
                w = position.width / elementCount;
                x = position.x + w * i;
                switch (i)
                {
                    case 0:
                        EditorGUIUtility.labelWidth = 25f;
                        EditorGUI.LabelField(new Rect(x, position.y, w, position.height), property.displayName);
                        break;
                    case 1:
                        EditorGUIUtility.labelWidth = 25f;
                        EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), property.FindPropertyRelative("Min"));
                        break;
                    case 2:
                        EditorGUIUtility.labelWidth = 25f;
                        EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), property.FindPropertyRelative("Max"));
                        break;
                }
            }
        }

        EditorGUI.EndProperty();
        EditorGUIUtility.labelWidth = 0f;
    }
}

#endif

[Serializable]
public struct IntRange
{
    public int Min;
    public int Max;

    public IntRange(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
