
using DG.Tweening;
using UnityEngine;

public class ChipInTrayState : ChipState
{
    private Camera cam;
    private Plane dragPlane;

    #region MouseFlick
    private Vector3 lastMousePosition;
    private Quaternion baseRotation;
    private float currentTilt;
    private Quaternion currentRot;
    private Transform meshTransform;
    #endregion

    public ChipInTrayState(ChipStateMachine stateMachine, ChipController chip, string animatorName) : base(stateMachine, chip, animatorName)
    {
        cam = Camera.main;
        meshTransform = chip.transform;
        Vector3 planeNormal = SceneViewManager.myChipView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myChipView.transform.position);
    }

    public override void Enter()
    {
        base.Enter();
        chip.rb.velocity = Vector3.zero;
        chip.rb.angularVelocity = Vector3.zero;
        chip.rb.useGravity = false;
        chip.col.isTrigger = true;
        
        chip.transform.position = chip.originalTransform.position;
        chip.transform.rotation = chip.originalTransform.rotation;
        baseRotation = meshTransform.localRotation;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        stateMachine.ChangeState(chip.dragState);
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();

        chip.outlineControl.Enable = 1f;

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            Vector3 mouseLocal = SceneViewManager.myChipView.transform.InverseTransformDirection(mouseWorld);
            lastMousePosition = mouseLocal;
            currentTilt = 0f;
            currentRot = Quaternion.identity;
        }
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        chip.outlineControl.Enable = 0f;

        meshTransform.DOLocalRotateQuaternion(baseRotation, 0.3f);
        currentTilt = 0f;
        currentRot = Quaternion.identity;
    }

    public override void OnMouseStay()
    {
        base.OnMouseStay();
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
            float targetTilt = Mathf.Clamp(mouseVelocity * 40, -10, 10);
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, 40 * dt);

            mouseDelta.z = -mouseDelta.z;   // 上下移动和左右移动的转向不一样
            Quaternion targetRot = Quaternion.identity;
            if (mouseVelocity > 0.1f)
            {
                Vector3 rotAxis = Quaternion.AngleAxis(-90f, Vector3.up) * mouseDelta;    // 从 handView 的坐标系转到 model 的坐标系下
                targetRot = Quaternion.AngleAxis(currentTilt, rotAxis);
            }
            currentRot = Quaternion.Slerp(currentRot, targetRot, 40 * dt);
            meshTransform.localRotation = currentRot * baseRotation;
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
