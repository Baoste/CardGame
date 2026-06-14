using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    public Transform PlayerTransform;
    public Transform camFreeTransform;

    public Vector3 offset;

    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 80.0f;

    private float rotationX = 0f;
    private float currentRotationY = 75f;

    private bool canLook = true;
    private bool skipOneFrame = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SyncRotationFromCamera();
    }

    private void LateUpdate()
    {
        if (!canLook)
            return;

        camFreeTransform.position = PlayerTransform.position + offset;

        if (skipOneFrame)
        {
            skipOneFrame = false;
            Input.ResetInputAxes();
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotationY += mouseX;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);

        PlayerTransform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
        camFreeTransform.rotation = Quaternion.Euler(rotationX, currentRotationY, 0f);
    }

    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;

        if (enabled)
        {
            SyncRotationFromCamera();
            skipOneFrame = true;
            Input.ResetInputAxes();
        }
    }

    public void SyncRotationFromCamera()
    {
        Vector3 euler = camFreeTransform.rotation.eulerAngles;

        currentRotationY = euler.y;

        rotationX = euler.x;
        if (rotationX > 180f)
            rotationX -= 360f;

        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);
    }
}