using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrap : MonoBehaviour
{
    [SerializeField] private TMP_Text logo;
    [SerializeField] private GameObject cam;

    void Start()
    {
        StartCoroutine(LoadFirstScene());
    }

    private IEnumerator LoadFirstScene()
    {
        logo.alpha = 0f;
        Sequence seq = DOTween.Sequence();
        seq.Append(logo.DOFade(1f, 0.5f));
        seq.AppendInterval(1.0f);
        seq.Append(logo.DOFade(0f, 0.5f));
        yield return seq.WaitForCompletion();
        logo.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(0.3f);
        Destroy(cam);
        SceneManager.LoadScene("gxz", LoadSceneMode.Additive);
    }

    public void SwitchToGameScene(string gameSceneName)
    {
        StartCoroutine(SwitchToGameSceneOnlyKeepBootstrap(gameSceneName));
    }

    private IEnumerator SwitchToGameSceneOnlyKeepBootstrap(string gameSceneName)
    {
        // 1. 卸载除了 BootstrapScene 和目标 GameScene 以外的场景
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == "BootstrapScene")
                continue;

            if (scene.name == gameSceneName)
                continue;

            if (scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        // 2. 如果目标场景还没加载，就 Additive 加载
        Scene gameScene = SceneManager.GetSceneByName(gameSceneName);

        if (!gameScene.IsValid() || !gameScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        }

        // 3. 设置目标场景为 Active Scene
        gameScene = SceneManager.GetSceneByName(gameSceneName);

        if (gameScene.IsValid() && gameScene.isLoaded)
        {
            SceneManager.SetActiveScene(gameScene);
        }
    }
}
