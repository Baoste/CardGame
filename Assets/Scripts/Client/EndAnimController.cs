using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimController : MonoBehaviour
{
    [SerializeField] private GameObject originalCharacter;
    [SerializeField] private Material postprocessMat;
    [SerializeField] private Renderer manChipRenderer;
    [SerializeField] private Renderer womanmanChipRenderer;

    [Header("Chip Spawner")]
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private int count = 100;
    [SerializeField] private int batch = 10;

    [SerializeField] private Vector3 spawnCenter;
    [SerializeField] private Vector3 spawnRange = new Vector3(5f, 0f, 5f);

    [SerializeField] private Material chipMaterial;
    
    private void Start()
    {
        int skinId = ChipSkinConfig.myAccountData.ChipAppearaceData.ChipSkinId;
        int colorId = ChipSkinConfig.myAccountData.ChipAppearaceData.ChipColorId;

        Material sourceMaterial = ChipSkinConfig.materials[skinId];

        chipMaterial = new Material(sourceMaterial);
        chipMaterial.enableInstancing = true;
        chipMaterial.SetTexture("_DecalTex", ChipSkinConfig.texture2Ds[colorId]);

        manChipRenderer.sharedMaterial = chipMaterial;
        womanmanChipRenderer.sharedMaterial = chipMaterial;

    }

    public void HideOriginalCharacter()
    {
        originalCharacter.SetActive(false);
    }

    public void StartEndAnim()
    {
        StartCoroutine(EndAnimCoroutine());
    }

    private IEnumerator EndAnimCoroutine()
    {
        postprocessMat.SetFloat("_Intensity", 0f);
        postprocessMat.SetFloat("_WhiteBloom", 0f);

        AudioManager.Instance.Play("EndFlash");

        Sequence endSeq = DOTween.Sequence();
        endSeq.Append(
            postprocessMat.DOFloat(1f, "_Intensity", 1f)
        );

        endSeq.Append(
            postprocessMat.DOFloat(1f, "_WhiteBloom", 0.2f)
        );

        yield return endSeq.WaitForCompletion();
        yield return new WaitForSeconds(5f);

        postprocessMat.SetFloat("_Intensity", 0f);
        FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("gxz");
    }


    public void SpawnChip()
    {
        StartCoroutine(_SpawnChip());
    }

    private IEnumerator _SpawnChip()
    {
        if (chipMaterial != null)
            chipMaterial.enableInstancing = true;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = spawnCenter + new Vector3(
                Random.Range(-spawnRange.x, spawnRange.x),
                Random.Range(-spawnRange.y, spawnRange.y),
                Random.Range(-spawnRange.z, spawnRange.z)
            );

            GameObject chip = Instantiate(chipPrefab, pos, Random.rotation, transform);

            Renderer renderer = chip.GetComponentInChildren<Renderer>();

            if (renderer != null && chipMaterial != null)
            {
                // 重点：用 sharedMaterial，不要用 material
                renderer.sharedMaterial = chipMaterial;
            }

            if (i % batch == 0 && i / batch != 0)
                yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position + spawnCenter;
        Vector3 size = spawnRange * 2f;

        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, 0.15f);
    }
}
