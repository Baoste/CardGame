using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelPreview : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60f;
    [SerializeField] private ChipViewController chipViewController;

    public ChipAppearaceData ChipAppearaceData;

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    public void SetChipColor(int chipColorId)
    {
        ChipAppearaceData.ChipColorId = chipColorId;
        chipViewController.ChangeMat(ChipAppearaceData);
    }

    public void SetChipSkin(int chipSkinId)
    {
        ChipAppearaceData.ChipSkinId = chipSkinId;
        chipViewController.ChangeMat(ChipAppearaceData);
    }
}
