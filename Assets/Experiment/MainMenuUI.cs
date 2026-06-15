using FishNet.Demo.AdditiveScenes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject chipUpdateCanvas;
    private Animator cameraAnimator;
    //public GameObject movingCamera;
    public ShotPlayer shotPlayer;
    public GameObject player;

    private void Awake()
    {
        //cameraAnimator = movingCamera.GetComponent<Animator>();
    }

    public void StartGame()
    {
        shotPlayer.PlayShot(1); // 自由相机

        mainMenuCanvas.SetActive(false); // 隐藏主菜单UI

        //player.GetComponent<PlayerController>().enabled = true;
        //player.GetComponent<PlayerMouseLook>().enabled = true;  // 启用玩家控制脚本
    }

    public void ShowClipUpdate()
    {
        mainMenuCanvas.SetActive(false);
        chipUpdateCanvas.SetActive(true);
    }

    public void HideClipUpdate()
    {
        chipUpdateCanvas.SetActive(false);

        if (shotPlayer.currentIndex == 0)
        {
            shotPlayer.PlayShot(1);
        }
    }

    public void ExitGame()
    {
        // 在这里添加退出游戏的逻辑
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
