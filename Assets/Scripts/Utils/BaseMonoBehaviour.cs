using System.Collections;
using System.Reflection;
using UnityEngine;

public class BaseMonoBehaviour : MonoBehaviour
{
    protected virtual void OnDestroy()
    {
        var info = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in info)
        {
            var fieldType = field.FieldType;

            if (typeof(IList).IsAssignableFrom(fieldType))
            {
                var list = field.GetValue(this) as IList;
                list?.Clear();
            }

            if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                var dictionary = field.GetValue(this) as IDictionary;
                dictionary?.Clear();
            }

            if (!fieldType.IsPrimitive)
            {
                field.SetValue(this, null);
            }
        }
    }
}