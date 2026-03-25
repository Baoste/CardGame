using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNoiseTilingOnClick : MonoBehaviour
{
    private Renderer targetRenderer;
    private Vector2 targetTiling = new Vector2(0f, 30f);

    private Material runtimeMat;
    private Vector2 originalTiling;
    private Vector2 currentTiling;

    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        runtimeMat = targetRenderer.material;
        originalTiling = runtimeMat.GetTextureScale("_NoiseTex");
        currentTiling = originalTiling;
    }

    private void OnMouseDown()
    {
        // Á¢¿ÌÇÐ»» tiling
        runtimeMat.SetTextureScale("_NoiseTex", targetTiling);

        Sequence seq = DOTween.Sequence();

        // ¶¶¶¯ 0.3 Ãë
        seq.Append(
            DOTween.To(
                () => currentTiling,
                x =>
                {
                    currentTiling = x;
                    runtimeMat.SetTextureScale("_NoiseTex", currentTiling);
                },
                targetTiling,
                0.1f
            ).SetEase(Ease.Flash)
        );

        // »Ö¸´
        seq.AppendCallback(() =>
        {
            runtimeMat.SetTextureScale("_NoiseTex", originalTiling);
            currentTiling = originalTiling;
        });
    }
}
