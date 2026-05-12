using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    public Transform PlayerTransform;

    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 80.0f;

    private float rotationX = 0;
    private float currentRotationY = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.position = PlayerTransform.position;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotationY += mouseX;
        PlayerTransform.rotation = Quaternion.Euler(0, currentRotationY, 0);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);

        transform.rotation = Quaternion.Euler(rotationX, currentRotationY, 0);
    }
}