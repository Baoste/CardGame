using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipView : MonoBehaviour
{
    [Header("Resolve Zone Size")]
    [SerializeField] private float zoneWidth = 6f;
    [SerializeField] private float zoneHeight = 2.5f;
    [SerializeField] private Transform cameraTransform;

    [Header("Drag Settings")]
    [SerializeField] private BoxCollider dragValidArea; // 安全区域，松手时如果不在这里就回位置

    [Header("Init Settings")]
    [SerializeField] private ChipInit chipInit;

    [HideInInspector] public List<GameObject> chipsInTray = new List<GameObject>();
    [HideInInspector] public List<GameObject> chipsPlaced = new List<GameObject>();

    public void GenerateChips(int count)
    {
        chipsInTray.AddRange(chipInit.GenerateChips(count, chipsInTray.Count));
    }

    public void DestroyChipsPlaced()
    {
        foreach (GameObject obj in chipsPlaced)
            Destroy(obj);
        chipsPlaced.Clear();
    }

    public void Place1Bet(GameObject chip)
    {
        chipsPlaced.Add(chip);
        chipsInTray.Remove(chip);
    }

    public IEnumerator Place1BetAuto(bool isOpponent)
    {
        if (chipsInTray.Count < 1) yield break;

        GameObject chip = chipsInTray[chipsInTray.Count - 1];
        chipsInTray.Remove(chip);
        chipsPlaced.Add(chip);

        Rigidbody rb = chip.GetComponentInChildren<Rigidbody>();
        Collider col = chip.transform.Find("Model/Chip/default").GetComponent<Collider>();
        rb.useGravity = false;
        col.isTrigger = true;

        Vector3 targetPos = isOpponent ? new Vector3(-1.369f, 1.93f, -1.185f) : new Vector3(-1.369f, 2.73f, -1.185f);

        Sequence seq = DOTween.Sequence();
        seq.Append(chip.transform.DOMove(chip.transform.position + Vector3.up * 0.2f, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.3f);
        seq.Append(chip.transform.DOLocalMove(targetPos, 0.5f));
        yield return seq.WaitForCompletion();

        rb.useGravity = true;
        col.isTrigger = false;
    }

    public bool IsOutsideValidArea(Vector3 worldPos)
    {
        if (dragValidArea == null)
            return false;

        Vector3 local = dragValidArea.transform.InverseTransformPoint(worldPos);
        Vector3 half = dragValidArea.size * 0.5f;

        return Mathf.Abs(local.x) > half.x ||
               Mathf.Abs(local.y) > half.y ||
               Mathf.Abs(local.z) > half.z;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        DrawRect(transform.position, transform.rotation, zoneWidth, zoneHeight);
        Gizmos.color = Color.gray;
        //Gizmos.DrawLine(instantiatePosition, instantiatePosition + -skillCardsDeck.up.normalized * dropDistance);
    }

    private void DrawRect(Vector3 center, Quaternion rotation, float width, float height)
    {
        Vector3 a = center + rotation * new Vector3(-width * 0.5f, 0f, -height * 0.5f);
        Vector3 b = center + rotation * new Vector3(width * 0.5f, 0f, -height * 0.5f);
        Vector3 c = center + rotation * new Vector3(width * 0.5f, 0f, height * 0.5f);
        Vector3 d = center + rotation * new Vector3(-width * 0.5f, 0f, height * 0.5f);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}
