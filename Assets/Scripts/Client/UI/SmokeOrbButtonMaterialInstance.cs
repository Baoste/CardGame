using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class SmokeOrbButtonMaterialInstance : MonoBehaviour
{
    [SerializeField] private Graphic targetGraphic;
    [SerializeField] private bool randomizeOnAwake = true;
    [SerializeField] private float randomSeed = 0f;

    private Material runtimeMaterial;

    private static readonly int RandomSeedId = Shader.PropertyToID("_RandomSeed");

    private void Awake()
    {
        if (targetGraphic == null)
            targetGraphic = GetComponent<Graphic>();

        // 重点：复制一份材质，而不是直接改 shared material
        runtimeMaterial = Instantiate(targetGraphic.material);
        runtimeMaterial.name = targetGraphic.material.name + " Runtime Instance";

        targetGraphic.material = runtimeMaterial;

        if (randomizeOnAwake)
            randomSeed = Random.Range(0f, 1000f);

        runtimeMaterial.SetFloat(RandomSeedId, randomSeed);
    }

    public void SetRandomSeed(float seed)
    {
        randomSeed = seed;

        if (runtimeMaterial != null)
            runtimeMaterial.SetFloat(RandomSeedId, randomSeed);
    }

    public void RandomizeSeed()
    {
        SetRandomSeed(Random.Range(0f, 1000f));
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}