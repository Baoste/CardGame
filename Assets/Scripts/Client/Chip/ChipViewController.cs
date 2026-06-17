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
        if (ChipSkinConfig.materials == null)
            return;

        Material[] mats = matRenderer.sharedMaterials;
        mats[0] = ChipSkinConfig.materials[chipAppearaceData.ChipSkinId];
        matRenderer.sharedMaterials = mats;

        Texture2D tex = ChipSkinConfig.texture2Ds[chipAppearaceData.ChipColorId];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_DecalTex", tex);
        matRenderer.SetPropertyBlock(mpb);
    }
}
