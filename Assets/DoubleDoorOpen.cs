using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorOpen : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H key pressed, opening doors...");
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Tween rotateTween1 = door1.transform.DOLocalRotate(
             new Vector3(0, -80, 0), // 目标旋转
             0.33f,
             RotateMode.LocalAxisAdd           // 增量旋转
         ).SetEase(Ease.Linear); 

        Tween rotateTween2 = door2.transform.DOLocalRotate(
             new Vector3(0, 80, 0), // 目标旋转
             0.33f,
             RotateMode.LocalAxisAdd           // 增量旋转
         ).SetEase(Ease.Linear); 
    }
}
