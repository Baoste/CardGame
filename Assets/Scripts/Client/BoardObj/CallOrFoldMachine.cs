using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOrFoldMachine : MonoBehaviour, IViewClear
{
    public bool isBack;

    [SerializeField] private Transform disk;
    private Vector3 diskOriginalPosition = new Vector3(-0.379f, 1.131f, -0.96f);

    private Quaternion originalRotation;
    
    private void Start()
    {
        disk.transform.localPosition = diskOriginalPosition;
        originalRotation = transform.rotation;
        gameObject.SetActive(false);
    }

    public void ClearView()
    {
        disk.transform.localPosition = diskOriginalPosition;
        transform.rotation = originalRotation;
    }

    public IEnumerator Show()
    {
        gameObject.SetActive(true);
        GameManager.ChangeInteractMask("Machine");

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(new Vector3(0, isBack ? 120 : -80, 0), 0.8f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce));
        seq.Append(disk.DOLocalMoveZ(-0.217f, 0.5f).SetEase(Ease.InOutCubic));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator Hide()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotateQuaternion(originalRotation, 0.8f).SetEase(Ease.OutBounce));
        seq.Append(disk.DOLocalMoveZ(diskOriginalPosition.z, 0.5f).SetEase(Ease.InOutCubic));

        yield return seq.WaitForCompletion();
        
        GameManager.ResetInteractMask();
        gameObject.SetActive(false);
    }
}
