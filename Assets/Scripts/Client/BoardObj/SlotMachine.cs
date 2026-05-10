using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public IEnumerator PlayAnimation(bool reveal)
    {
        Quaternion quaternion = transform.rotation;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(Vector3.zero, 0.8f).SetEase(Ease.OutBounce));
        yield return seq.WaitForCompletion();

        // TODO: 显示随机引爆装置爆炸动画
        if (reveal)
            yield return new WaitForSecondsRealtime(1.5f);
        else
            yield return new WaitForSecondsRealtime(1.5f);

        transform.rotation = quaternion;
    }
}
