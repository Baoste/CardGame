using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCardResolve : CardInstance
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

    public override void InitCardInstance(int cardId, int instanceId)
    {
        base.InitCardInstance(cardId, instanceId);

        localScaleFactor = 0.45f;
        pointText.text = point.ToString();

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_BaseMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

}
