using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    public void SetCardTexture_None(int point)
    {
        pointText.text = point.ToString();
        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardState.None);

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    public void SetCardTexture_Hole(int point)
    {
        pointText.text = "";
        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardState.Hole);

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    public void SetCardTexture_Hidden()
    {
        pointText.text = "";

        matRenderer.materials = CardViewCreator.Instance.pointCardStateMatMap.Get(CardState.Hidden);
    }
}
