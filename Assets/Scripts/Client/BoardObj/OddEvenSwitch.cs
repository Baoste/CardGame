using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddEvenSwitch : MonoBehaviour, IMouseDrag, IMouseUp
{
    [HideInInspector] public bool chosenDone;
    [HideInInspector] public int chosenId;

    private Vector3 basePivot;
    private Vector3 baseForward;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        basePivot = transform.position;
        baseForward = -transform.forward;
        chosenDone = false;
        chosenId = -1;
    }

    public void MouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(-baseForward, basePivot + 0.1f * baseForward);

        Vector3 hitPoint = Vector3.zero;
        if (plane.Raycast(ray, out float enter))
        {
            hitPoint = ray.GetPoint(enter);
        }

        Vector3 dir3 = hitPoint - basePivot;

        // 投影到 YZ 平面
        Vector2 dirYZ = new Vector2(dir3.y, dir3.z);
        // basePivot 的 z 轴方向（forward）
        Vector2 zAxisYZ = new Vector2(baseForward.y, baseForward.z);

        float angle = 0f;

        if (dirYZ.sqrMagnitude > 0.0001f && zAxisYZ.sqrMagnitude > 0.0001f)
        {
            dirYZ.Normalize();
            zAxisYZ.Normalize();
            angle = Vector2.SignedAngle(zAxisYZ, dirYZ);
        }

        angle = Mathf.Clamp(angle, -60, 60);
        transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    public void MouseUp()
    {
        if (Mathf.Abs(transform.localEulerAngles.x) > 45)
        {
            StartCoroutine(CloseSwitch());
        }
        else
        {
            transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
        }

    }

    private IEnumerator CloseSwitch()
    {
        float angleX = transform.localEulerAngles.x;
        if (angleX > 180) angleX -= 360;
        float sign = Mathf.Sign(angleX);
        transform.DOLocalRotate(new Vector3(80, 0, 0) * sign, 0.1f).SetEase(Ease.OutCubic);
        chosenId = sign > 0 ? 1 : 0;
        yield return new WaitForSeconds(0.15f);
        chosenDone = true;
    }
}
