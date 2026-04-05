using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLightView : MonoBehaviour, IViewClear
{
    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    private void Start()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void ClearView()
    {
        SetLight(0);
    }

    public void SetLight(int turn)
    {
        matRenderer.GetPropertyBlock(mpb);

        if (turn == 1 || turn == 4)
        {
            float t = 0f;
            DOTween.To(() => t, x =>
            {
                t = x;
                mpb.SetFloat("_FlashFactor", t);
                matRenderer.SetPropertyBlock(mpb);

            }, 1f, 1f).SetEase(Ease.InCubic);
        }

        switch (turn)
        {
            case 0:
                mpb.SetVector("_LightColor", new Vector3(1, 1, 1));
                mpb.SetFloat("_IsBreath", 0f);
                mpb.SetVector("_LightControl1", Vector3.zero);
                mpb.SetVector("_LightControl2", Vector3.zero);
                break;
            case 1:
                mpb.SetVector("_LightControl1", new Vector3(1, 1, 0));
                break;
            case 2:
                mpb.SetVector("_LightControl1", new Vector3(1, 1, 1));
                break;
            case 3:
                mpb.SetVector("_LightControl2", new Vector3(1, 0, 0));
                break;
            case 4:
                mpb.SetVector("_LightControl2", new Vector3(1, 1, 0));
                // Ñ­»·ºôÎü
                mpb.SetVector("_LightColor", new Vector3(1, 0, 0));
                mpb.SetFloat("_IsBreath", 1f);
                break;
            //case 5:
            //    mpb.SetVector("_LightControl2", new Vector3(1, 1, 0));
            //    break;
            default:
                break;
        }
        matRenderer.SetPropertyBlock(mpb);
    }
}
