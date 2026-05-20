using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUpAnim : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        yield return new WaitForSecondsRealtime(2f);
        transform.DOMoveY(transform.position.y + 5.0f, 1.0f).SetEase(Ease.OutQuad);
    }
}
