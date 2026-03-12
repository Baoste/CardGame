using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardMouseTilt : MonoBehaviour
{
    [SerializeField] private float tiltStrength = 20f;
    [SerializeField] private float maxTilt = 60f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private bool useUnscaledDelta = true;
    [SerializeField] private Transform meshTransform;

    private Vector3 lastMousePosition;
    private Quaternion baseRotation;

    private float currentTilt;
    private Quaternion currentRot;

    private void Start()
    {
        baseRotation = meshTransform.localRotation;
    }

    public void InitTilt(Vector3 mousePosition)
    {
        lastMousePosition = mousePosition;
        currentTilt = 0f;
        currentRot = Quaternion.identity;
    }

    public void Tilt(Vector3 mousePosition)
    {
        Vector3 currentMousePosition = mousePosition;
        Vector3 mouseDelta = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;

        float dt = useUnscaledDelta ? Time.unscaledDeltaTime : Time.deltaTime;
        if (dt <= 0f) return;

        // 鼠标速度
        float mouseVelocity = mouseDelta.magnitude / dt;
        float targetTilt = Mathf.Clamp(mouseVelocity * tiltStrength, -maxTilt, maxTilt);
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, smoothSpeed * dt);

        mouseDelta.z = -mouseDelta.z;   // 上下移动和左右移动的转向不一样
        Vector3 rotAxis = Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up) * mouseDelta;    // 从 handView 的坐标系转到 model 的坐标系下
        Quaternion targetRot = Quaternion.AngleAxis(currentTilt, rotAxis);
        currentRot = Quaternion.Slerp(currentRot, targetRot, smoothSpeed * dt);
        meshTransform.localRotation = currentRot * baseRotation;
    }

    public void ResetBaseRotation()
    {
        meshTransform.localRotation = baseRotation;
        currentTilt = 0f;
        currentRot = Quaternion.identity;
    }
}