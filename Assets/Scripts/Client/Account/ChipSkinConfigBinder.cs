using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSkinConfigBinder : MonoBehaviour
{
    [SerializeField] private Texture2D[] texture2Ds;
    [SerializeField] private Material[] materials;

    private void Awake()
    {
        ChipSkinConfig.texture2Ds = texture2Ds;
        ChipSkinConfig.materials = materials;

        Debug.Log($"ChipSkinConfig loaded. Textures={texture2Ds.Length}, Materials={materials.Length}");
    }
}
