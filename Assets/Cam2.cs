using FishNet.Example.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam2 : MonoBehaviour
{
    public ShotPlayer shotPlayer;
    public GameObject player;
    //public GameObject freeCam;

    public void SwitchFreeCam()
    {
        shotPlayer.PlayShot(2); // 切换到自由摄像机
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerMouseLook>().enabled = true;
    }
}
