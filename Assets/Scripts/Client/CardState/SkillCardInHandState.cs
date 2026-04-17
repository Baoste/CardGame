
using DG.Tweening;
using Game.Domain;
using UnityEngine;

public class SkillCardInHandState : SkillCardState
{
    private float hoveredMoveDirY = 0.4f;

    public SkillCardInHandState(SkillCardStateMachine stateMachine, SkillCardController skillCard, string animatorName) : base(stateMachine, skillCard, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        skillCard.instance.meshRenderer.sharedMaterial = skillCard.instance.defaultMaterial;
        skillCard.outlineControl.OutlineColor = skillCard.outlineControl.defaultColor;
        skillCard.transform.localScale = Vector3.one * skillCard.instance.localScaleFactor;
        skillCard.UpdateCardPosition();
    }

    public override void Exit()
    {
        base.Exit();
        skillCard.instance.meshRenderer.sharedMaterial = skillCard.instance.defaultMaterial;
        skillCard.outlineControl.OutlineColor = skillCard.outlineControl.defaultColor;
        skillCard.transform.localScale = Vector3.one * skillCard.instance.localScaleFactor;
        CameraMouseLook.Locked = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        stateMachine.ChangeState(skillCard.dragState);
    }

    public override void OnMouseDrag()
    {
        base.OnMouseDrag();
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (skillCard.isOpponent) return;

        skillCard.outlineControl.Enable = 1f;
        skillCard.transform.DOScale(Vector3.one * skillCard.instance.localScaleFactor * 2.0f, 0.15f);

        Vector3 newPos = skillCard.instance.originalPos;
        newPos.y = skillCard.instance.originalPos.y + hoveredMoveDirY;
        skillCard.transform.position = newPos;
        skillCard.transform.DOMoveY(newPos.y + 0.1f, 0.5f).SetEase(Ease.OutCubic);

        CameraMouseLook.Locked = true;
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        if (skillCard.isOpponent) return;

        skillCard.outlineControl.Enable = 0f;
        skillCard.transform.DOScale(Vector3.one * skillCard.instance.localScaleFactor, 0.15f);

        Vector3 newPos = skillCard.instance.originalPos;
        newPos.y = skillCard.instance.originalPos.y + 0.1f;
        skillCard.transform.position = newPos;
        skillCard.transform.DOMoveY(skillCard.instance.originalPos.y, 0.5f).SetEase(Ease.OutCubic);

        CameraMouseLook.Locked = false;
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();
    }

    public override void Update()
    {
        base.Update();
    }
}
