using System.Collections;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private Material glitchMaterial;

    [Header("Glitch Params")]
    [SerializeField] private float defaultIntensity = 0f;
    [SerializeField] private float triggerIntensity = 0.7f;
    [SerializeField] private float duration = 0.25f;

    private Coroutine glitchCoroutine;

    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");

    private void Awake()
    {
        // Make sure the glitch is disabled by default.
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat(IntensityId, defaultIntensity);
        }
    }

    public void TriggerGlitch()
    {
        TriggerGlitch(triggerIntensity, duration);
    }

    public void TriggerGlitch(float intensity, float time)
    {
        if (glitchMaterial == null) return;

        // Restart current glitch if triggered again.
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
        }

        glitchCoroutine = StartCoroutine(GlitchRoutine(intensity, time));
    }

    private IEnumerator GlitchRoutine(float intensity, float time)
    {
        glitchMaterial.SetFloat(IntensityId, intensity);

        yield return new WaitForSeconds(time);

        glitchMaterial.SetFloat(IntensityId, defaultIntensity);

        glitchCoroutine = null;
    }

    public void SetGlitchIntensity(float intensity)
    {
        if (glitchMaterial == null) return;

        // External systems can directly control the post-process strength.
        glitchMaterial.SetFloat(IntensityId, intensity);
    }

    public void StopGlitch()
    {
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            glitchCoroutine = null;
        }

        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat(IntensityId, defaultIntensity);
        }
    }
}