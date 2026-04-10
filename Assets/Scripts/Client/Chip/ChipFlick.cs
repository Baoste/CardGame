using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ChipFlick : MonoBehaviour, IMouseEnter, IMouseStay, IMouseExit
{
    private Camera cam;
    private Plane dragPlane;
    private Outline outlineControl;

    [SerializeField] private float tiltStrength = 20f;
    [SerializeField] private float maxTilt = 40f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Transform meshTransform;

    private Vector3 lastMousePosition;
    private Quaternion baseRotation;
    private float currentTilt;
    private Quaternion currentRot;

    private ChipDraggable draggable;

    private void Start()
    {
        cam = Camera.main;
        outlineControl = GetComponent<Outline>();
        baseRotation = meshTransform.localRotation;
        currentTilt = 0f;
        currentRot = Quaternion.identity;

        Vector3 planeNormal = SceneViewManager.myChipView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myChipView.transform.position);
    }

    public void MouseEnter()
    {
        outlineControl.Enable = 1f;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            lastMousePosition = mouseLocal;
            currentTilt = 0f;
            currentRot = Quaternion.identity;
        }
    }

    public void MouseStay()
    {
        draggable = GetComponent<ChipDraggable>();
        if (draggable == null || draggable.executed || draggable.IsDragging) return;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);

            // mouseTilt.Tilt(mouseLocal);
            Vector3 currentMousePosition = mouseLocal;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f) return;

            // 鼠标速度
            float mouseVelocity = mouseDelta.magnitude / dt;
            float targetTilt = Mathf.Clamp(mouseVelocity * tiltStrength, -maxTilt, maxTilt);
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, smoothSpeed * dt);

            mouseDelta.z = -mouseDelta.z;   // 上下移动和左右移动的转向不一样
            Quaternion targetRot = Quaternion.identity;
            if (mouseVelocity > 0.1f)
            {
                Vector3 rotAxis = Quaternion.AngleAxis(-90f, Vector3.up) * mouseDelta;    // 从 handView 的坐标系转到 model 的坐标系下
                targetRot = Quaternion.AngleAxis(currentTilt, rotAxis);
            }
            currentRot = Quaternion.Slerp(currentRot, targetRot, smoothSpeed * dt);
            meshTransform.localRotation = currentRot * baseRotation;
        }
    }

    public void MouseExit()
    {
        outlineControl.Enable = 0f;

        meshTransform.DOLocalRotateQuaternion(baseRotation, 0.3f);
        currentTilt = 0f;
        currentRot = Quaternion.identity;
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

