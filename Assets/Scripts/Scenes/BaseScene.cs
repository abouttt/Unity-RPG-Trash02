using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class BaseScene : BaseMonoBehaviour
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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Managers.Clear();
    }
}
