using System;
using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class BaseScene : BaseMonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; private set; } = SceneType.Unknown;

    [SerializeField]
    private AudioClip _sceneBGM;

    private int _currentLabelIndex = 0;

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

        Managers.Data.LoadSettings();
    }

    protected void InstantiatePackage(string packageName)
    {
        var package = Managers.Resource.Instantiate(packageName);
        package.transform.DetachChildren();
        Destroy(package);
    }

    protected void LoadResourcesAsync(SceneType sceneType, Action callback = null)
    {
        var loadResourceLabels = SceneSettings.GetInstance.LoadResourceLabels[sceneType];
        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            return;
        }

        Managers.Resource.LoadAllAsync<UnityEngine.Object>(loadResourceLabels[_currentLabelIndex].ToString(), () =>
        {
            if (_currentLabelIndex == loadResourceLabels.Length - 1)
            {
                _currentLabelIndex = 0;
                callback?.Invoke();
            }
            else
            {
                _currentLabelIndex++;
                LoadResourcesAsync(sceneType, callback);
            }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Managers.Clear();
    }
}
