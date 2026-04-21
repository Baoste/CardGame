using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCardViewController : MonoBehaviour
{
    [Header("Component")]
    public TMP_Text pointText;

    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public IEnumerator ChangeCardTexture_None(int point)
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
}
