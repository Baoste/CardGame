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
            Sequence seq = DOTween.Sequence();
            // 加速阶段
            seq.Append(
                transform.DORotate(
                    new Vector3(0, 120, 0),
                    2f,
                    RotateMode.LocalAxisAdd
                ).SetEase(Ease.InSine)
            );
            // 加速结束后，开启匀速循环
            seq.AppendCallback(() =>
            {
                rotateTween = transform.DORotate(
                    new Vector3(0, 360, 0),
                    5f,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            });
        }
        else
        {
            rotateTween.Kill();
        }
    }
}
