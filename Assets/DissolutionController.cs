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
    //public Material material;

    public bool isDissolving = false;
    public float dissolveSpeed = 1f;
    private float dissolveAmount = 5f;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        r = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }
    void Update()
    {
        r.GetPropertyBlock(mpb);
        //mpb.SetVector("_Center", target.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
            );
        }
        r.SetPropertyBlock(mpb);
    }
}
