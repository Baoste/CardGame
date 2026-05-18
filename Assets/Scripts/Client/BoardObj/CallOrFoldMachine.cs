using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CallOrFoldMachine : MonoBehaviour, IViewClear
{
    public bool isBack;

    [SerializeField] private Transform disk;
    private Vector3 diskOriginalPosition = new Vector3(-0.379f, 1.131f, -0.96f);
    [SerializeField] private ClickToCallOrFold callBtn;
    [SerializeField] private TMP_Text betCountText;

    private Vector3 originalPosition;

    [Header("Volume Control")]
    [SerializeField] private Volume volume;

    private ColorAdjustments colorAdjust;

    private void Start()
    {
        volume.profile.TryGet(out colorAdjust);
        disk.transform.localPosition = diskOriginalPosition;
        originalPosition = transform.localPosition;
        gameObject.SetActive(false);
    }

    public void ClearView()
    {
        disk.transform.localPosition = diskOriginalPosition;
        transform.localPosition = originalPosition;
    }

    public IEnumerator Show(int betCount)
    {
        if (callBtn != null)
            callBtn.betCount = betCount;
        if (betCountText != null)
            betCountText.text = betCount.ToString();

        gameObject.SetActive(true);
        HighLight(gameObject);
        GameManager.ChangeInteractMask("HighlightOnly");

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(isBack ? -3.11f : 2.016f, 0.8f).SetEase(Ease.OutBounce));
        seq.Append(disk.DOLocalMoveZ(-0.217f, 0.5f).SetEase(Ease.InOutCubic));
        seq.Append(disk.DOLocalRotate(new Vector3(0f, 10f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InCubic));
        seq.AppendCallback(() =>
        {
            disk.DOLocalRotate(new Vector3(0f, 360f, 0f), 6f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        });

        yield return seq.WaitForCompletion();
    }

    public IEnumerator Hide()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(originalPosition.x, 0.8f).SetEase(Ease.OutBounce));
        seq.Append(disk.DOLocalMoveZ(diskOriginalPosition.z, 0.5f).SetEase(Ease.InOutCubic));

        yield return seq.WaitForCompletion();
        
        GameManager.ResetInteractMask();
        CancelHighLight(gameObject);
        gameObject.SetActive(false);
    }

    private void HighLight(GameObject obj)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            -100,
            0.5f
        );

        int layer = LayerMask.NameToLayer("HighlightOnly");
        SetLayerRecursively(obj, layer);
    }

    private void CancelHighLight(GameObject obj)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            0,
            0.5f
        );

        int layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(obj, layer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
