using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShaderTest : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public Volume globalVolume;

    private bool selecting = false;

    void Start()
    {
        StartCoroutine(ClickTest());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!selecting)
            {
                selecting = true;
                gameObjects[0].layer = LayerMask.NameToLayer("HighlightOnly");
                gameObjects[1].layer = LayerMask.NameToLayer("HighlightOnly");
                globalVolume.profile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustments);
                DOTween.To(
                    () => colorAdjustments.saturation.value,
                    x => colorAdjustments.saturation.value = x,
                    -90f,
                    1f
                );
            }
            else
            {
                selecting = false;
                gameObjects[0].layer = LayerMask.NameToLayer("Default");
                gameObjects[1].layer = LayerMask.NameToLayer("Default");
                globalVolume.profile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustments);
                DOTween.To(
                    () => colorAdjustments.saturation.value,
                    x => colorAdjustments.saturation.value = x,
                    0f,
                    1f
                );
            }
        }
    }

    private IEnumerator ClickTest()
    {
        while (true)
        {
            yield return null;
            if (!Input.GetMouseButtonDown(0))
                continue;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                continue;
            VFXPlayer player = hit.collider.GetComponentInParent<VFXPlayer>();
            if (!player)
                continue;

            player.PlayMusicRhythmVFX();
            Debug.Log("1");
            break;
        }
    }
}
