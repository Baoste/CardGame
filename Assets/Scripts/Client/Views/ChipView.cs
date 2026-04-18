using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Transform chipContainer;

    [HideInInspector] public Dictionary<int, GameObject> chipsInTray = new Dictionary<int, GameObject>();
    [HideInInspector] public Dictionary<int, GameObject> chipsPlaced = new Dictionary<int, GameObject>();

    private void Start()
    {
        chipContainer.Rotate(Vector3.forward, 180);
    }

    public void StartGame(bool isOpponent)
    {
        chipInit.GenerateChips(6, isOpponent, ref chipsInTray);
        chipContainer.DORotate(new Vector3(0, 0, 180f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
    }

    public void GenerateChips(int count, bool isOpponent)
    {
        chipInit.GenerateChips(count, isOpponent, ref chipsInTray);
    }

    public void DestroyChipsPlaced()
    {
        foreach (var obj in chipsPlaced.Values)
        {
            Destroy(obj);
        }
        chipsPlaced.Clear();
    }

    public void ReturnCard(int id, GameObject obj)
    {
        chipsPlaced.Remove(id);
        chipsInTray[id] = obj;
    }

    public void Place1Bet(int id)
    {
        chipsPlaced[id] = chipsInTray[id];
        chipsInTray.Remove(id);
    }

    public IEnumerator Place1BetAuto(bool isOpponent, float delay)
    {
        if (chipsInTray.Count < 1) yield break;

        yield return new WaitForSecondsRealtime(delay);

        int id = chipsInTray.Keys.Last();
        GameObject chip = chipsInTray[id];

        chipsPlaced[id] = chipsInTray[id];
        chipsInTray.Remove(id);

        Rigidbody rb = chip.GetComponentInChildren<Rigidbody>();
        Collider col = chip.transform.Find("Model/Chip/default").GetComponent<Collider>();
        rb.useGravity = false;
        col.isTrigger = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(chip.transform.DOMove(chip.transform.position + Vector3.up * 0.2f, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.3f);
        seq.Append(chip.transform.DOMove(transform.position, 0.5f));
        yield return seq.WaitForCompletion();

        ChipController chipController = chip.GetComponentInChildren<ChipController>();
        chipController.stateMachine.ChangeState(chipController.placedState);
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
