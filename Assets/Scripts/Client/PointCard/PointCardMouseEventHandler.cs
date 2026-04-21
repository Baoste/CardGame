using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardMouseEventHandler : MonoBehaviour, IMouseEnter, IMouseExit, IMouseDown, IMouseDrag, IMouseUp
{
    private PointCardController pointCard;

    public void Init()
    {
        pointCard = GetComponent<PointCardController>();
    }

    public void MouseDown()
    {
        pointCard.stateMachine.currentState.OnMouseDown();
    }

    public void MouseDrag()
    {
        pointCard.stateMachine.currentState.OnMouseDrag();
    }
    public void MouseUp()
    {
        pointCard.stateMachine.currentState.OnMouseUp();
    }

    public void MouseEnter()
    {
        pointCard.stateMachine.currentState.OnMouseEnter();
    }

    public void MouseExit()
    {
        pointCard.stateMachine.currentState.OnMouseExit();
    }
}
