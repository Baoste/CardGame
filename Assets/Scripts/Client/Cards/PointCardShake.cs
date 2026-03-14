using DG.Tweening;
using GameKit.Dependencies.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardShake : MonoBehaviour
{
    private float shakeHeight = 0.03f;
    public void CardShake()
    {
        Quaternion originalRotation = transform.rotation;

        Sequence wobble = DOTween.Sequence();
        wobble.Append(transform.DOLocalRotate(new Vector3(12, 0, 0), 0.08f, RotateMode.LocalAxisAdd));
        wobble.Append(transform.DOLocalRotate(new Vector3(-12, 0, 0), 0.08f, RotateMode.LocalAxisAdd));
        wobble.Append(transform.DOLocalRotate(new Vector3(6, 0, 0), 0.06f, RotateMode.LocalAxisAdd));
        wobble.Append(transform.DOLocalRotate(new Vector3(-3, 0, 0), 0.05f, RotateMode.LocalAxisAdd));
        wobble.Append(transform.DORotateQuaternion(originalRotation, 0.05f));

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOJump(transform.position, shakeHeight, 1, 0.2f));
        seq.Join(wobble);
    }
}
