using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour, IViewClear
{
    [HideInInspector] public float rotateTime = 0.5f;

    public void ClearView()
    {
        transform.DORotate(Vector3.zero, rotateTime).SetEase(Ease.OutBack);
    }

    public void Rotate2Player(bool isOpponent)
    {
        if (isOpponent)
        {
            transform.DORotate(new Vector3(0, 180, 0), rotateTime).SetEase(Ease.OutBack);
        }
        else
        {
            transform.DORotate(Vector3.zero, rotateTime).SetEase(Ease.OutBack);
        }
    }
}
