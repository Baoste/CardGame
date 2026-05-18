using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLightView : MonoBehaviour, IViewClear
{
    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    [SerializeField] private List<Transform> lightStrips;
    private Dictionary<Transform, Vector3> lightStripMap;

    private void Start()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();

        lightStripMap = new Dictionary<Transform, Vector3>();
        foreach (var strip in lightStrips)
        {
            lightStripMap[strip] = strip.localPosition;
        }
    }

    public void ClearView()
    {
        SetLight(0);
        foreach (var strip in lightStripMap.Keys)
        {
            strip.localPosition = lightStripMap[strip];
        }
    }

    public void SetLight(int turn)
    {
        matRenderer.GetPropertyBlock(mpb);
        switch (turn)
        {
            case 0:
                mpb.SetVector("_LightColor", new Vector3(1, 1, 1));
                mpb.SetFloat("_IsBreath", 0f);
                mpb.SetVector("_LightControl1", Vector3.zero);
                mpb.SetVector("_LightControl2", Vector3.zero);
                break;
            case 1:
                mpb.SetVector("_LightControl1", new Vector3(1, 0, 0));
                break;
            case 2:
                mpb.SetVector("_LightControl1", new Vector3(1, 1, 0));
                break;
            case 3:
                mpb.SetVector("_LightControl1", new Vector3(1, 1, 1));
                break;
            case 4:
                mpb.SetVector("_LightControl2", new Vector3(1, 0, 0));
                break;
            case 5:
                mpb.SetVector("_LightControl2", new Vector3(1, 1, 0));
                // Ñ­»·ºôÎü
                mpb.SetVector("_LightColor", new Vector3(1, 0, 0));
                mpb.SetFloat("_IsBreath", 1f);
                // Î»ÖÃ·É³ö
                foreach (var strip in lightStripMap.Keys)
                {
                    strip.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
                }
                break;
            default:
                break;
        }
        matRenderer.SetPropertyBlock(mpb);

        //if (turn == 1 || turn == 4)
        //{
        //    float t = 0f;
        //    DOTween.To(() => t, x =>
        //    {
        //        t = x;
        //        matRenderer.GetPropertyBlock(mpb);
        //        mpb.SetFloat("_FlashFactor", t);
        //        matRenderer.SetPropertyBlock(mpb);

        //    }, 2f, 1f).SetEase(Ease.InCubic);
        //}

    }
}
