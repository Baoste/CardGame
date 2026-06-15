using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipInit : MonoBehaviour
{
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Vector3 instantiatePosition;
    [SerializeField] private float spacing;

    public void GenerateChips(int count, bool isOpponent, ref Dictionary<int, GameObject> chips)
    {
        Vector3 worldPos = transform.TransformPoint(transform.localPosition + instantiatePosition);
        int baseId = isOpponent ? 600 : 500;

        int addCount = 0, i = 0;
        while (addCount < count)
        {
            int k = baseId + i;
            if (chips.ContainsKey(k))
            {
                i++;
                continue;
            }
            GameObject chip = Instantiate(chipPrefab, transform);
            chip.transform.position = worldPos + transform.right * i * spacing;
            chip.transform.rotation = Quaternion.Euler(0, 14.9f, -86f);

            ChipController chipController = chip.GetComponentInChildren<ChipController>();
            chipController.instanceId = k;
            chipController.originalTransform = chip.transform;

            if (ChipSkinConfig.Instance != null)
            {
                if (!isOpponent)
                {
                    ChipViewController chipViewController = chip.GetComponentInChildren<ChipViewController>();
                    chipViewController.ChangeMat(ChipSkinConfig.Instance.myAccountData.ChipAppearaceData);
                }
                else
                {
                    ChipViewController chipViewController = chip.GetComponentInChildren<ChipViewController>();
                    chipViewController.ChangeMat(ChipSkinConfig.Instance.opponentAccountData.ChipAppearaceData);
                }
            }

            chips[k] = chip;
            i++;
            addCount++;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 worldPos = transform.TransformPoint(transform.localPosition + instantiatePosition);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(worldPos, 0.02f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(worldPos, worldPos + transform.right * 0.5f);
    }
}
