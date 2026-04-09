
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

public class SkillCardDragState : SkillCardState
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

    public SkillCardDragState(SkillCardStateMachine stateMachine, SkillCard skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
        cam = Camera.main;
        meshTransform = skillCard.transform.Find("Model");
        Vector3 planeNormal = SceneViewManager.myHandView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myHandView.transform.position);
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        skillCard.transform.DOKill();

        if (TryGetMouseWorldPosition(out Vector3 mouseWorld))
        {
            skillCard.transform.position = mouseWorld;
            // Mouse Tilt
            Vector3 mouseLocal = SceneViewManager.myHandView.transform.InverseTransformDirection(mouseWorld);
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
            skillCard.transform.position = mouseWorld;
            // Mouse Tilt
            Vector3 currentMousePosition = SceneViewManager.myHandView.transform.InverseTransformDirection(mouseWorld);
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f) return;

            // 鼠标速度
            float mouseVelocity = mouseDelta.magnitude / dt;
            float targetTilt = Mathf.Clamp(mouseVelocity * 30, -40, 40);
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, 10 * dt);

            mouseDelta.z = -mouseDelta.z;   // 上下移动和左右移动的转向不一样
            Quaternion targetRot = Quaternion.identity;
            if (mouseVelocity > 0.01f)
            {
                Vector3 rotAxis = Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up) * mouseDelta;    // 从 handView 的坐标系转到 model 的坐标系下
                targetRot = Quaternion.AngleAxis(currentTilt, rotAxis);
            }
            currentRot = Quaternion.Slerp(currentRot, targetRot, 10 * dt);
            meshTransform.localRotation = currentRot * baseRotation;


            // ===== 检测是否离开合法区域 =====
            bool outside = SceneViewManager.myHandView != null && SceneViewManager.myHandView.IsOutsideValidArea(skillCard.transform.position);

            if (outside)
            {
                skillCard.outlineControl.OutlineColor = skillCard.outlineControl.outAreaColor;
            }
            else
            {
                skillCard.outlineControl.OutlineColor = skillCard.outlineControl.defaultColor;
            }
        }
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();


        bool shouldRemove = SceneViewManager.myHandView != null && SceneViewManager.myHandView.IsOutsideValidArea(skillCard.transform.position);
        if (shouldRemove)
        {
            skillCard.StartExecuteCard();
        }
        else
        {
            stateMachine.ChangeState(skillCard.inHandState);
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
