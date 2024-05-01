using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; private set; } = SceneType.Unknown;

    [SerializeField]
    private AudioClip _sceneBGM;

    private void Awake()
    {
        Managers.Init();
        Init();
    }

    protected virtual void Init()
    {
        if (FindObjectOfType(typeof(EventSystem)) == null)
        {
            Managers.Resource.Instantiate("EventSystem.prefab");
        }
    }

    private void OnDestroy()
    {
        Managers.Clear();
    }
}
