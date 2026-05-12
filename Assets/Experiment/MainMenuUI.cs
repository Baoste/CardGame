using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    private Animator cameraAnimator;
    //public GameObject movingCamera;
    public ShotPlayer shotPlayer;

    private void Awake()
    {
        //cameraAnimator = movingCamera.GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public void StartGame()
    {
        // 在这里添加开始游戏的逻辑
        Debug.Log("开始游戏");

        shotPlayer.PlayShot(1); // 下楼梯

        mainMenuCanvas.SetActive(false); // 隐藏主菜单UI
    }

    public void ExitGame()
    {
        // 在这里添加退出游戏的逻辑
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
