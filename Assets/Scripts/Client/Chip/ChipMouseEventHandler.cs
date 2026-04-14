using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipMouseEventHandler : MonoBehaviour, IMouseEnter, IMouseStay, IMouseExit, IMouseDown, IMouseDrag, IMouseUp
{
    private ChipController chip;

    public void Init()
    {
        chip = GetComponent<ChipController>();
    }

    public void MouseDown()
    {
        chip.stateMachine.currentState.OnMouseDown();
    }

    public void MouseDrag()
    {
        chip.stateMachine.currentState.OnMouseDrag();
    }
    public void MouseUp()
    {
        chip.stateMachine.currentState.OnMouseUp();
    }

    public void MouseEnter()
    {
        chip.stateMachine.currentState.OnMouseEnter();
    }

    public void MouseStay()
    {
        chip.stateMachine.currentState.OnMouseStay();
    }

    public void MouseExit()
    {
        chip.stateMachine.currentState.OnMouseExit();
    }
}
