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

    public List<GameObject> chipsPlaced;

    public void Place1Bet(GameObject chip)
    {
        chipsPlaced.Add(chip);
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
        //Gizmos.color = Color.gray;
        //Gizmos.DrawSphere(instantiatePosition, 0.02f);
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
