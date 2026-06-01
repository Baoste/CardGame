using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PointCardViewController : MonoBehaviour
{
    [Header("Component")]
    public TMP_Text pointText;

    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    private float clipFactor = 1f;

    private void Awake()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void ChangeCardTexture(CardVisualState cardState, int point)
    {
        switch (cardState)
        {
            case CardVisualState.None:
                StartCoroutine(ChangeCardTexture_None(point));
                break;
            case CardVisualState.Hidden:
                StartCoroutine(ChangeCardTexture_Hidden());
                break;
        }
    }

    private IEnumerator ChangeCardTexture_None(int point)
    {
        GetComponent<PointCardController>().smokeVFX.Play();
        yield return new WaitForSeconds(0.5f);

        pointText.text = point.ToString();
        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardVisualState.None);

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    private IEnumerator ChangeCardTexture_Hidden()
    {
        GetComponent<PointCardController>().smokeVFX.Play();
        yield return new WaitForSeconds(0.5f);
    }

    public void SetCardTexture_None(int point)
    {
        pointText.text = point.ToString();
        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardVisualState.None);

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    public void SetCardTexture_Hole(int point)
    {
        pointText.text = "";
        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardVisualState.Hole);

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    public void SetCardTexture_Hidden()
    {
        pointText.text = "";

        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardVisualState.Hidden);
    }

    public void SetCardTexture_Resolve(int point)
    {
        pointText.text = "";
        PointCardResolve resolve = GetComponent<PointCardResolve>();

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_BaseMap", tex);
        matRenderer.SetPropertyBlock(mpb);

        Sequence seq = DOTween.Sequence();

        seq.Append(DOTween.To(
            () => clipFactor,
            value =>
            {
                clipFactor = value;
                matRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_ClipFactor", clipFactor);
                matRenderer.SetPropertyBlock(mpb);
            },
            0f,
            1f
        ));
        seq.Join(
            transform.DOScale(Vector3.one * 0.5f, 0.7f)
                .SetEase(Ease.OutBack)
        );

        seq.Insert(
            0.7f + 0.1f,
            transform.DOScale(Vector3.one * 0.45f, 0.1f)
                .SetEase(Ease.OutQuad)
        );
        seq.InsertCallback(0.8f, () =>
        {
            resolve.smokeVFX.Play();
        });

        seq.AppendCallback(() =>
        {
            pointText.text = point.ToString();
        });
    }
}
