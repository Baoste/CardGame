using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlaySceneAnim : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCamera;

    private void Start()
    {
        //AudioManager.Instance.Play("BGM");
        AudioManager.Instance.Play("Electric_Buzz");
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (GameBootstrap.isDebugMode)
            StartCoroutine(StartDebug());
        else
            StartCoroutine(StartAnim());
    }

    private IEnumerator StartAnim()
    {
        startCamera.Priority = -1; // 切换到开始动画的摄像机
        yield return null;
        
        int seed = MatchData.Instance.matchSeed;
        ClientCommand.StartMatch(seed);
    }

    private IEnumerator StartDebug()
    {
        startCamera.Priority = -1; // 切换到开始动画的摄像机
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());
        SceneViewManager.myChipView.StartGame(false);
        SceneViewManager.opponentChipView.StartGame(true);
        foreach (var obj in SceneViewManager.myChipView.chipsInTray.Values)
        {
            ChipMouseEventHandler drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipMouseEventHandler>();
            drag.Init();
        }
    }
}
