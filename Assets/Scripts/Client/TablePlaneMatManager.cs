using DG.Tweening;
using GameKit.Dependencies.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TablePlaneMatManager : MonoBehaviour
{
    [Serializable]
    public class MaterialEntry
    {
        public string name;
        public Material material;
    }

    [SerializeField] private List<MaterialEntry> entries;
    [SerializeField] private Renderer targetRenderer;

    private Dictionary<string, Material> materials = new();

    private MaterialPropertyBlock mpb;
    private float strength;

    private void Awake()
    {
        foreach (var entry in entries)
        {
            materials[entry.name] = entry.material;
        }
        mpb = new MaterialPropertyBlock();
    }

    public void SetFirstMaterial(string matName)
    {
        Material mat = materials[matName];

        Material[] mats = targetRenderer.materials;
        if (mats.Length == 1)
        {
            Material oldMat = mats[0];
            targetRenderer.materials = new Material[]
            {
                mat,
                oldMat
            };
        }
        else
        {
            mats[0] = mat;
            targetRenderer.materials = mats;
        }
    }

    public void PlayPlaneAnim(float showTime, float delay, float dispearTime)
    {
        SetStrength(-1f);

        DOTween.Sequence()
            .Append(DOTween.To(
                () => strength,
                value => SetStrength(value),
                1f,
                showTime
            ))
            .Append(DOTween.To(
                () => strength,
                value => SetStrength(value),
                2f,
                delay
            ))
            .Append(DOTween.To(
                () => strength,
                value => SetStrength(value),
                -1f,
                dispearTime
            ))
            .AppendCallback(() =>
            {
                Material[] mats = targetRenderer.materials;
                if (mats.Length > 1)
                {
                    targetRenderer.materials = new Material[] { mats[1] };
                }
            });
    }

    private void SetStrength(float value)
    {
        strength = value;

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Strength", strength);
        targetRenderer.SetPropertyBlock(mpb);
    }
}
