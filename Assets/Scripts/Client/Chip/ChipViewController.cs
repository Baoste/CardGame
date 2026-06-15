using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipViewController : MonoBehaviour
{
    [Header("Component")]
    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void ChangeMat(ChipAppearaceData chipAppearaceData)
    {
        Material[] mats = matRenderer.materials;
        mats[0] = ChipSkinConfig.Instance.materials[chipAppearaceData.ChipSkinId];
        matRenderer.materials = mats;

        Texture2D tex = ChipSkinConfig.Instance.texture2Ds[chipAppearaceData.ChipColorId];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_BaseMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }
}
