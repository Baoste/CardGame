using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelPreview : MonoBehaviour
{
    [SerializeField] private ChipViewController chipViewController;
    [SerializeField] private GameObject chipViewCanvas;
    public ChipAppearaceData ChipAppearaceData;

    [Header("Drag")]
    [SerializeField] private float dragSensitivity = 0.3f;
    // 鼠标每移动 1 像素，转多少度

    [Header("Idle Rotation")]
    [SerializeField] private float idleRotateSpeed = 3f;

    [Header("Inertia")]
    [SerializeField] private float inertiaDamping = 5f;
    // 惯性衰减速度，越大停得越快
    [SerializeField] private float minAngularVelocity = 0.01f;
    // 小于这个速度时直接停止，避免无限微小旋转
    private bool isDragging;
    private Vector3 lastMousePos;
    // 当前惯性角速度，单位：度/秒
    private float angularVelocity;

    private float idleAngularVelocity;


    private void Update()
    {
        if (chipViewCanvas.activeInHierarchy)
        {
            HandleMouseInput();
            ApplyRotation();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 只有鼠标在屏幕左半边按下时才允许拖动
            if (Input.mousePosition.x <= Screen.width * 0.5f)
            {
                isDragging = true;
                lastMousePos = Input.mousePosition;
                angularVelocity = 0f;
                idleAngularVelocity = 0f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 mouseDelta = currentMousePos - lastMousePos;

            // 横向拖动控制 Z 轴旋转
            float rotateDelta = -mouseDelta.x * dragSensitivity;

            transform.Rotate(Vector3.forward, rotateDelta);

            if (Time.deltaTime > 0f)
            {
                angularVelocity = rotateDelta / Time.deltaTime;
            }

            lastMousePos = currentMousePos;
        }
    }

    private void ApplyRotation()
    {
        if (isDragging)
            return;

        // 有拖拽惯性时，优先使用惯性
        if (Mathf.Abs(angularVelocity) > minAngularVelocity)
        {
            transform.Rotate(Vector3.forward, angularVelocity * Time.deltaTime);

            angularVelocity = Mathf.Lerp(
                angularVelocity,
                0f,
                inertiaDamping * Time.deltaTime
            );

            idleAngularVelocity = 0f;
            return;
        }

        // 惯性结束
        angularVelocity = 0f;

        // 待机自转从 0 慢慢加速到 idleRotateSpeed
        idleAngularVelocity = Mathf.MoveTowards(
            idleAngularVelocity,
            idleRotateSpeed,
            8f * Time.deltaTime
        );

        transform.Rotate(Vector3.forward, idleAngularVelocity * Time.deltaTime);
    }

    public void SetChipColor(int chipColorId)
    {
        ChipAppearaceData.ChipColorId = chipColorId;
        chipViewController.ChangeMat(ChipAppearaceData);
    }

    public void SetChipSkin(int chipSkinId)
    {
        ChipAppearaceData.ChipSkinId = chipSkinId;
        chipViewController.ChangeMat(ChipAppearaceData);
    }
}
