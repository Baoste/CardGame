using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseRaycastSystem : MonoBehaviour
{
    private Camera rayCamera;
    private LayerMask interactMask;
    private GameObject currentObj; // 当前 hover
    private GameObject dragObj;    // 当前拖拽对象

    private void Start()
    {
        rayCamera = Camera.main;
        interactMask = LayerMask.GetMask("Card", "Default");
    }

    private void Update()
    {
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);

        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, 1000f, interactMask);

        // -------------------------
        // Hover Enter / Exit
        // -------------------------
        if (hasHit)
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj != currentObj)
            {
                if (currentObj != null)
                {
                    var exit = currentObj.GetComponent<IMouseExit>();
                    exit?.MouseExit();
                }

                currentObj = hitObj;

                var enter = currentObj.GetComponent<IMouseEnter>();
                enter?.MouseEnter();
            }
        }
        else
        {
            if (currentObj != null)
            {
                var exit = currentObj.GetComponent<IMouseExit>();
                exit?.MouseExit();
                currentObj = null;
            }
        }

        // -------------------------
        // Mouse Down
        // -------------------------
        if (Input.GetMouseButtonDown(0))
        {
            if (hasHit)
            {
                dragObj = hit.collider.gameObject;

                var down = dragObj.GetComponent<IMouseDown>();
                down?.MouseDown();

                var click = dragObj.GetComponent<IMouseClick>();
                click?.MouseClick();
            }
        }

        // -------------------------
        // Mouse Drag
        // -------------------------
        if (Input.GetMouseButton(0))
        {
            if (dragObj != null)
            {
                var drag = dragObj.GetComponent<IMouseDrag>();
                drag?.MouseDrag();
            }
        }

        // -------------------------
        // Mouse Up
        // -------------------------
        if (Input.GetMouseButtonUp(0))
        {
            if (dragObj != null)
            {
                var up = dragObj.GetComponent<IMouseUp>();
                up?.MouseUp();
                dragObj = null;
            }
        }
    }
}
