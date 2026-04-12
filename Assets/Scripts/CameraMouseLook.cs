using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    [Header("Angle")]
    [SerializeField] private float maxYaw = 5f;
    [SerializeField] private float maxPitch = 3f;

    [Header("Spring")]
    [SerializeField] private float stiffness = 8f;
    [SerializeField] private float damping = 6f;

    [Header("Dead Zone")]
    [SerializeField, Range(0.3f, 0.8f)] private float deadZone = 0.5f;

    private Quaternion _initialRot;
    private Vector2 _currentOffset;
    private Vector2 _velocity;

    private void Start()
    {
        _initialRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float y = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Vector2 rawInput = new Vector2(x, y);

        // 死区处理
        Vector2 targetOffset = ApplyDeadZone(rawInput, deadZone);

        // 弹簧阻尼
        Vector2 force = (targetOffset - _currentOffset) * stiffness;
        _velocity += force * Time.deltaTime;
        _velocity *= Mathf.Exp(-damping * Time.deltaTime);
        _currentOffset += _velocity * Time.deltaTime;

        float yaw = _currentOffset.x * maxYaw;
        float pitch = -_currentOffset.y * maxPitch;

        Quaternion targetRot = _initialRot * Quaternion.Euler(pitch, yaw, 0f);
        transform.localRotation = targetRot;
    }

    // ⭐ 核心：死区 + 平滑映射
    private Vector2 ApplyDeadZone(Vector2 input, float dz)
    {
        return new Vector2(
            ProcessAxis(input.x, dz),
            ProcessAxis(input.y, dz)
        );
    }

    private float ProcessAxis(float v, float dz)
    {
        float abs = Mathf.Abs(v);

        // 在死区内 → 直接0
        if (abs < dz)
            return 0f;

        // 超出死区 → 重映射到 [0,1]
        float sign = Mathf.Sign(v);
        float t = (abs - dz) / (1f - dz);

        // 可选：让边缘更柔一点（手感更丝滑）
        t = t * t; // 你也可以改成 Mathf.SmoothStep(0,1,t)

        return sign * t;
    }
}