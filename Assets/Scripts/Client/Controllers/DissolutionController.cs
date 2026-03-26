using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Time;

//[ExecuteAlways]
public class DissolutionController : MonoBehaviour
{
    private Renderer r;
    public Transform target;

    public bool isDissolving = false;
    public float dissolveSpeed = 1f;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        r = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void DestroySelf()
    {
        r.GetPropertyBlock(mpb);

        float dissolveAmount = 0f;

        DOTween.To(
            () => dissolveAmount,
            x =>
            {
                dissolveAmount = x;

                mpb.SetFloat("_DissolutionStrength", dissolveAmount);
                r.SetPropertyBlock(mpb);
            },
            1f,                         // 目标值：1
            1f / dissolveSpeed          // 时间
        )
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
