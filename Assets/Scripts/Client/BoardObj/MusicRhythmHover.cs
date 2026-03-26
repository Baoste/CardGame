using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRhythmHover : MonoBehaviour, IMouseEnter, IMouseExit
{
    public Renderer rhythm;
    public Renderer text;

    private void Start()
    {
        rhythm.enabled = false;
        text.enabled = false;
    }

    public void MouseEnter()
    {
        rhythm.enabled = true;
        text.enabled = true;
    }

    public void MouseExit()
    {
        rhythm.enabled = false;
        text.enabled = false;
    }
}
