using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipCover : MonoBehaviour, IMouseDown
{
    public void MouseDown()
    {
        StartCoroutine(SceneViewManager.viewAnimController.CloseChipCover());
    }
}
