using FishNet.Example.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam2 : MonoBehaviour
{
    public ShotPlayer shotPlayer;
    public GameObject player;
    public GameObject mainMenuCanvas;

    public GameObject SingleDoor_Open;
    public GameObject SingleDoor_Closed;
    //public GameObject freeCam;

    //public void SwitchFreeCam()
    //{
    //    shotPlayer.PlayShot(2); // 切换到自由摄像机
    //    player.GetComponent<PlayerController>().enabled = true;
    //    player.GetComponent<PlayerMouseLook>().enabled = true;

    //}

    public void MainMenuActivate()
    {
        mainMenuCanvas.SetActive(true);
        SingleDoor_Open.SetActive(false);
        SingleDoor_Closed.SetActive(true);
    }
}
