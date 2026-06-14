using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 5f;
    [Header("重力")]
    public float gravity = -9.81f;          // 重力加速度
    public float groundCheckDistance = 3f; // 地面检测距离

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ---- 地面检测 ----
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f)
        {
            // 贴地时把下落速度归零，防止微小负值累积
            verticalVelocity = -2f; // 保持一个小负值确保 isGrounded 稳定
        }

        // ---- 水平输入 ----
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * inputX + transform.forward * inputZ;

        // 限制斜向速度不至于过快
        if (move.magnitude > 1f)
            move.Normalize();

        // 应用水平移动（CharacterController 负责处理碰撞）
        controller.Move(move * moveSpeed * Time.deltaTime);

        // ---- 重力 ----
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}