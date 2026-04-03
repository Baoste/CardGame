using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipDraggable : MonoBehaviour, IMouseDown, IMouseDrag, IMouseUp
{
    private Camera cam;
    private bool isDragging;
    private Plane dragPlane;

    public bool IsDragging => isDragging;
    public bool executed = false;

    private ChipMouseTilt mouseTilt;
    private Outline outlineControl;
    private Rigidbody rb;
    [SerializeField] private Collider col;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        cam = Camera.main;
        mouseTilt = GetComponent<ChipMouseTilt>();
        outlineControl = GetComponent<Outline>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    public void MouseDown()
    {
        isDragging = true;
        transform.DOKill();
        rb.useGravity = false;
        col.isTrigger = true;

        Vector3 planeNormal = SceneViewManager.myChipView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myChipView.transform.position);

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            // 转到 handView 的坐标系下
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.InitTilt(mouseLocal);
        }
    }

    public void MouseDrag()
    {
        if (executed) return;
        if (!isDragging || cam == null) return;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            transform.position = mouseWorld;
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            mouseTilt.Tilt(mouseLocal);

            // ===== 新增：检测是否离开合法区域 =====
            bool outside = SceneViewManager.myChipView != null && SceneViewManager.myChipView.IsOutsideValidArea(transform.position);

            if (outside)
            {
                outlineControl.OutlineColor = outlineControl.outAreaColor;
            }
            else
            {
                // instance.meshRenderer.sharedMaterial = instance.defaultMaterial;
                outlineControl.OutlineColor = outlineControl.defaultColor;
            }
        }
    }

    public void MouseUp()
    {
        if (executed) return;
        if (!isDragging) return;
        isDragging = false;
        rb.useGravity = true;
        col.isTrigger = false;

        bool outside = SceneViewManager.myChipView != null && SceneViewManager.myChipView.IsOutsideValidArea(transform.position);

        if (outside)
        {
            // StartCoroutine(ReturnToHand());
        }
        else
        {
            SceneViewManager.myChipView.Place1Bet(gameObject);
            Place1BetCommand cmd = new Place1BetCommand { playerId = ClientGameState.playerSlot };
            ClientGameState.gateway.SendCommandServerRpc("Place1Bet", JsonConvert.SerializeObject(cmd));
        }
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            worldPos = ray.GetPoint(enter);
            return true;
        }

        return false;
    }
}
