using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject mainMenuCanvas;
    public GameObject chipUpdateCanvas;
    public GameObject loginCanvas;

    [Header("Account")]
    public AccountChipCountText accountChipCountText;
    public AccountLeaderboardText cccountLeaderboardText;

    // private Animator cameraAnimator;
    // public GameObject movingCamera;
    [Header("Other")]
    public ShotPlayer shotPlayer;
    public GameObject player;

    [SerializeField] private Material postprocessMat;
    [SerializeField] private RawImage rawImage;

    private void Awake()
    {
        if (GameBootstrap.isLogin)
        {

            mainMenuCanvas.SetActive(false);
            chipUpdateCanvas.SetActive(false);
            loginCanvas.SetActive(false);

            Sequence seq = DOTween.Sequence();
            seq.Append(postprocessMat.DOFloat(0f, "_WhiteBloom", 0.3f));
            seq.AppendInterval(2.5f);
            seq.Append(
                postprocessMat.DOFloat(1f, "_Intensity", 0.05f)
                    .SetEase(Ease.OutQuad)
            );
            seq.Append(
                postprocessMat.DOFloat(0f, "_Intensity", 0.18f)
                    .SetEase(Ease.OutQuad)
            );
            seq.AppendCallback(() =>
            {
                RefreshAccountData();
                AudioManager.Instance.PlayBGM("ComputerRoomBGM");
            });
        }

        //cameraAnimator = movingCamera.GetComponent<Animator>();
    }

    private void RefreshAccountData()
    {
        cccountLeaderboardText.RefreshLeaderboard();
        accountChipCountText.RefreshChipCount();
    }

    public void ShowMenu()
    {
        loginCanvas.SetActive(false);

        AudioManager.Instance.PlayBGM("MenuBGM");

        mainMenuCanvas.SetActive(true);
        rawImage.color = Color.black;
        Sequence seq = DOTween.Sequence();
        seq.Append(
            rawImage.DOColor(Color.white, 1)
        );
    }

    public void StartGame()
    {
        GameBootstrap.isLogin = true;

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM("ComputerRoomBGM");
        RefreshAccountData();

        shotPlayer.PlayShot(1); // 自由相机

        Sequence seq = DOTween.Sequence();
        seq.Append(
            rawImage.DOColor(Color.black, 1.5f)
        );
        seq.Append(
            rawImage.DOFade(0f, 1.5f)
        );
        seq.OnComplete(() =>
        {
            mainMenuCanvas.SetActive(false); // 隐藏主菜单UI
        });

        //player.GetComponent<PlayerController>().enabled = true;
        //player.GetComponent<PlayerMouseLook>().enabled = true;  // 启用玩家控制脚本
    }

    public void ShowClipUpdate()
    {
        AudioManager.Instance.PlayBGM("ClipUpdateBGM");
        accountChipCountText.RefreshChipCount();
        loginCanvas.SetActive(false);
        chipUpdateCanvas.SetActive(true);
    }

    public void HideClipUpdate()
    {
        chipUpdateCanvas.SetActive(false);

        if (shotPlayer.currentIndex == 0)
        {
            mainMenuCanvas.SetActive(true);
            rawImage.color = Color.black;
            Sequence seq = DOTween.Sequence();
            seq.Append(
                rawImage.DOColor(Color.white, 1)
            );
        }
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM("MenuBGM");
    }

    public void ExitGame()
    {
        // 在这里添加退出游戏的逻辑
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
