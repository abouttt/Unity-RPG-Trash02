using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EnumType;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private Image _background;

    [SerializeField]
    private Image _loadingBar;

    private int _currentLabelIndex;

    protected override void Init()
    {
        base.Init();

        _background.sprite = SceneSettings.GetInstance.BackgroundImages[Managers.Scene.NextScene];
        _background.color = Color.white;
        if (_background.sprite == null)
        {
            _background.color = Color.black;
        }

        Managers.Resource.Clear();
        LoadResourcesAsync(Managers.Scene.NextScene, () => StartCoroutine(LoadSceneAsync()));
    }

    private IEnumerator LoadSceneAsync()
    {
        var op = SceneManager.LoadSceneAsync(Managers.Scene.NextScene.ToString());
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.unscaledDeltaTime;
            if (op.progress < 0.9f)
            {
                _loadingBar.fillAmount = Mathf.Lerp(op.progress, 1f, timer);
                if (_loadingBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _loadingBar.fillAmount = Mathf.Lerp(_loadingBar.fillAmount, 1f, timer);
                if (_loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
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
}
