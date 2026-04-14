using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipDragState : ChipState
{
    private Plane dragPlane;
    private Camera cam;

    #region MouseTilt
    private Vector3 lastMousePosition;
    private Quaternion baseRotation;
    private float currentTilt;
    private Quaternion currentRot;
    private Transform meshTransform;
    #endregion

    public ChipDragState(ChipStateMachine stateMachine, ChipController chip, string animatorName) : base(stateMachine, chip, animatorName)
    {
        cam = Camera.main;
        meshTransform = chip.transform;
        Vector3 planeNormal = SceneViewManager.myChipView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myChipView.transform.position);
    }

    public override void Enter()
    {
        base.Enter();
        baseRotation = meshTransform.localRotation;
    }

    public override void Exit()
    {
        base.Exit();
        meshTransform.localRotation = baseRotation;
        currentTilt = 0f;
        currentRot = Quaternion.identity;
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        chip.outlineControl.Enable = 1f;
        chip.transform.DOKill();

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            chip.transform.position = mouseWorld;
            // Mouse Tilt
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            lastMousePosition = mouseLocal;
            currentTilt = 0f;
            currentRot = Quaternion.identity;
        }
    }

    public override void OnMouseDrag()
    {
        base.OnMouseDrag();
        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            chip.transform.position = mouseWorld;
            Vector3 currentMousePosition = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f) return;

            // 鼠标速度
            float mouseVelocity = mouseDelta.magnitude / dt;
            float targetTilt = Mathf.Clamp(mouseVelocity * 20, -40, 40);
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, 10 * dt);

            mouseDelta.z = -mouseDelta.z;   // 上下移动和左右移动的转向不一样
            Quaternion targetRot = Quaternion.identity;
            if (mouseVelocity > 0.01f)
            {
                Vector3 rotAxis = Quaternion.AngleAxis(-90f, Vector3.up) * mouseDelta;    // 从 handView 的坐标系转到 model 的坐标系下
                targetRot = Quaternion.AngleAxis(currentTilt, rotAxis);
            }
            currentRot = Quaternion.Slerp(currentRot, targetRot, 10 * dt);
            meshTransform.localRotation = currentRot * baseRotation;

            // ===== 检测是否离开合法区域 =====
            bool outside = SceneViewManager.myChipView != null && SceneViewManager.myChipView.IsOutsideValidArea(chip.transform.position);

            if (outside)
            {
                chip.outlineControl.OutlineColor = chip.outlineControl.outAreaColor;
            }
            else
            {
                chip.outlineControl.OutlineColor = chip.outlineControl.defaultColor;
            }
        }
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();
        chip.outlineControl.Enable = 0f;

        bool shouldRemove = SceneViewManager.myChipView != null && SceneViewManager.myChipView.IsOutsideValidArea(chip.transform.position);
        if (shouldRemove)
        {
            stateMachine.ChangeState(chip.inTrayState);
        }
        else
        {
            chip.StartExecuteChip();
        }
    }

    public override void Update()
    {
        base.Update();
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
