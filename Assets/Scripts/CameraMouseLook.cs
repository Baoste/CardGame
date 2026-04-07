using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    [Header("Angle")]
    [SerializeField] private float maxYaw = 5f;    // 左右
    [SerializeField] private float maxPitch = 3f;  // 上下

    [Header("Parameter")]
    [SerializeField] private float stiffness = 8f;
    [SerializeField] private float damping = 6f;

    private Quaternion _initialRot;
    private Vector2 _currentOffset;
    private Vector2 _velocity;

    private void Start()
    {
        _initialRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        // 鼠标转
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float y = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Vector2 targetOffset = new Vector2(x, y);

        //弹簧阻尼
        Vector2 force = (targetOffset - _currentOffset) * stiffness;
        _velocity += force * Time.deltaTime;
        _velocity *= Mathf.Exp(-damping * Time.deltaTime);
        _currentOffset += _velocity * Time.deltaTime;

        // 转角度
        float yaw = _currentOffset.x * maxYaw;
        float pitch = -_currentOffset.y * maxPitch;

        Quaternion targetRot = _initialRot * Quaternion.Euler(pitch, yaw, 0f);

        // 旋转
        transform.localRotation = targetRot;
    }
}