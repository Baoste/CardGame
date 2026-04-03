using UnityEngine;

public class ChipHover : MonoBehaviour, IMouseEnter, IMouseExit
{
    private Outline outlineControl;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        outlineControl = GetComponent<Outline>();
    }

    public void MouseEnter()
    {
        outlineControl.Enable = 1f;
    }

    public void MouseExit()
    {
        outlineControl.Enable = 0f;
    }
}
