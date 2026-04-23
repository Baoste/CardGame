using UnityEngine;

public class MouseRaycastSystem : MonoBehaviour
{
    private Camera rayCamera;
    private LayerMask interactMask;

    private GameObject currentObj; // 当前 hover
    private GameObject dragObj;    // 当前拖拽对象

    // hover 缓存接口
    private IMouseEnter currentEnter;
    private IMouseExit currentExit;
    private IMouseStay currentStay;

    // drag 缓存接口
    private IMouseDown dragDown;
    private IMouseClick dragClick;
    private IMouseDrag dragDrag;
    private IMouseUp dragUp;

    private void Start()
    {
        rayCamera = Camera.main;
        interactMask = LayerMask.GetMask("Card", "HighlightOnly", "Default");
    }

    private void Update()
    {
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, 1000f, interactMask);

        HandleHover(hasHit, hit);
        HandleMouseDown(hasHit, hit);
        HandleMouseDrag();
        HandleMouseUp();
    }

    private void HandleHover(bool hasHit, RaycastHit hit)
    {
        if (hasHit)
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj != currentObj)
            {
                ClearCurrentHover();

                currentObj = hitObj;
                CacheHoverInterfaces(currentObj);

                currentEnter?.MouseEnter();
            }
            else
            {
                currentStay?.MouseStay();
            }
        }
        else
        {
            ClearCurrentHover();
        }
    }

    private void HandleMouseDown(bool hasHit, RaycastHit hit)
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (hasHit)
        {
            dragObj = hit.collider.gameObject;
            CacheDragInterfaces(dragObj);

            dragDown?.MouseDown();
            dragClick?.MouseClick();
        }
    }

    private void HandleMouseDrag()
    {
        if (!Input.GetMouseButton(0)) return;

        if (dragObj != null)
        {
            dragDrag?.MouseDrag();
        }
    }

    private void HandleMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;

        if (dragObj != null)
        {
            dragUp?.MouseUp();
            ClearDrag();
        }
    }

    private void CacheHoverInterfaces(GameObject obj)
    {
        currentEnter = obj.GetComponent<IMouseEnter>();
        currentExit = obj.GetComponent<IMouseExit>();
        currentStay = obj.GetComponent<IMouseStay>();
    }

    private void CacheDragInterfaces(GameObject obj)
    {
        dragDown = obj.GetComponent<IMouseDown>();
        dragClick = obj.GetComponent<IMouseClick>();
        dragDrag = obj.GetComponent<IMouseDrag>();
        dragUp = obj.GetComponent<IMouseUp>();
    }

    private void ClearCurrentHover()
    {
        if (currentObj != null)
        {
            currentExit?.MouseExit();
        }

        currentObj = null;
        currentEnter = null;
        currentExit = null;
        currentStay = null;
    }

    private void ClearDrag()
    {
        dragObj = null;
        dragDown = null;
        dragClick = null;
        dragDrag = null;
        dragUp = null;
    }
}