using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    private Animator cameraAnimator;
    public GameObject movingCamera;

    public void Awake()
    {
        cameraAnimator = movingCamera.GetComponent<Animator>();
    }
    public void StartGame()
    {
        // 在这里添加开始游戏的逻辑，例如加载游戏场景
        Debug.Log("开始游戏");
        cameraAnimator.SetTrigger("PlayIntro"); // 播放摄像机动画
        mainMenuCanvas.SetActive(false); // 隐藏主菜单UI
    }

    public void ExitGame()
    {
        // 在这里添加退出游戏的逻辑
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
