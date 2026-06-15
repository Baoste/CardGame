using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSkinConfig : Singleton<ChipSkinConfig>
{
    public Texture2D[] texture2Ds;
    public Material[] materials;

    [HideInInspector] public ChipAppearaceData chipAppearaceData;
}
