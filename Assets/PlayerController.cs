using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform camFreeTransform;

    private Vector3 moveDirection = Vector3.zero;
    private bool isJumping = false;

    void Start()
    {
    }

    void FixedUpdate()
    {
        // 获取输入
        float inputX = Input.GetAxis("Horizontal"); // A/D
        float inputZ = Input.GetAxis("Vertical");   // W/S

        // 移动向量，基于自身朝向
        Vector3 move = transform.right * inputX + transform.forward * inputZ;

        // 应用移动
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}