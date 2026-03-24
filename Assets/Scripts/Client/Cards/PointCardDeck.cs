using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardDeck : MonoBehaviour
{
    private Tween rotateTween;

    void Start()
    {
        ChangeRotateState(true);
    }

    public void ChangeRotateState(bool isStart)
    {
        if (isStart)
        {
            rotateTween = transform.DORotate(
                new Vector3(0, 360, 0),
                8f,
                RotateMode.LocalAxisAdd
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        }
        else
        {
            rotateTween.Kill();
        }
    }
}
