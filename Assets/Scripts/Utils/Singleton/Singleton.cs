using UnityEngine;

public class Singleton<T> : BaseMonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;

    public static T GetInstance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<T>();
                if (s_instance == null)
                {
                    new GameObject($"{typeof(T).Name}", typeof(T));
                }
            }

            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (s_instance == this)
        {
            s_instance = null;
        }
    }
}
