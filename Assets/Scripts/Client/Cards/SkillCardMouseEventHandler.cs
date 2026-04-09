using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardMouseEventHandler : MonoBehaviour, IMouseEnter, IMouseExit, IMouseDown, IMouseDrag, IMouseUp
{
    private SkillCard skillCard;

    public void Init()
    {
        skillCard = GetComponent<SkillCard>();
    }

    public void MouseDown()
    {
        skillCard.stateMachine.currentState.OnMouseDown();
    }

    public void MouseDrag()
    {
        skillCard.stateMachine.currentState.OnMouseDrag();
    }
    public void MouseUp()
    {
        skillCard.stateMachine.currentState.OnMouseUp();
    }

    public void MouseEnter()
    {
        skillCard.stateMachine.currentState.OnMouseEnter();
    }

    public void MouseExit()
    {
        skillCard.stateMachine.currentState.OnMouseExit();
    }
}
