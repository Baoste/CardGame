using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimController : MonoBehaviour
{
    [SerializeField] private Material postprocessMat;

    public void StartEndAnim()
    {
        StartCoroutine(EndAnimCoroutine());
    }

    private IEnumerator EndAnimCoroutine()
    {
        postprocessMat.SetFloat("_Intensity", 0f);
        postprocessMat.SetFloat("_WhiteBloom", 0f);

        AudioManager.Instance.Play("EndFlash");

        Sequence endSeq = DOTween.Sequence();
        endSeq.Append(
            postprocessMat.DOFloat(1f, "_Intensity", 1f)
        );

        endSeq.Append(
            postprocessMat.DOFloat(1f, "_WhiteBloom", 0.2f)
        );

        yield return endSeq.WaitForCompletion();
        yield return new WaitForSeconds(5f);

        postprocessMat.SetFloat("_Intensity", 0f);
        FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("gxz");
    }
}
