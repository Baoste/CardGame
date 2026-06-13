using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public class RevealButton : MonoBehaviour, IMouseEnter, IMouseExit, IMouseClick
{
    private Vector3 originalPosition;  // 存储原始位置
    private Quaternion originalRotation;  // 存储原始旋转角度
    public GameObject buttonLight;  // 按钮的模型对象

    // 设置晃动幅度
    public float shakeAmount = 0.1f;
    public float shakeDuration = 0.2f;

    // 设置按钮按下时的下沉幅度
    public float pressAmount = 0.05f;
    public float pressDuration = 0.1f;

    // 设置倾斜的最大角度
    public float maxTiltAngle = 15f;
    public float tiltSpeed = 0.1f;

    private bool isMouseOver = false; // 标记鼠标是否悬停在按钮上

    [SerializeField] private Transform pivot;

    public bool hasClicked = true;

    private void Start()
    {
        // originalPosition = transform.position;  // 获取按钮的初始位置
        originalRotation = transform.rotation;  // 保存按钮的初始旋转
    }

    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
    }

    // 鼠标进入时，按钮轻微晃动并开启按钮灯光
    public void MouseEnter()
    {
        if (hasClicked) return;
        isMouseOver = true;  // 标记鼠标进入
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        TiltButton(mouseWorldPos);  // 根据鼠标位置进行倾斜
        buttonLight.SetActive(true);  // 打开按钮灯光
    }

    // 鼠标退出时，按钮回到原位置并恢复原始旋转
    public void MouseExit()
    {
        if (hasClicked) return;
        isMouseOver = false;  // 标记鼠标离开
        transform.DOKill();  // 停止所有动画
        transform.DOLocalMove(originalPosition, shakeDuration);  // 回到原始位置
        ResetTilt();  // 恢复按钮的倾斜
        buttonLight.SetActive(false);  // 关闭按钮灯光
    }

    // 鼠标按下时，按钮下沉并倾斜
    public void MouseClick()
    {
        // 教程特定
        if (ClientGameState.IsTutorial)
        {
            ClientGameState.TutorialStepDone = true;
            hasClicked = true;
            TiltButton(GetMouseWorldPosition());  // 根据鼠标位置进行倾斜
            buttonLight.SetActive(false);  // 关闭按钮灯光
            transform.DOLocalMove(originalPosition - Vector3.up * pressAmount, pressDuration).SetEase(Ease.InQuad);
            return;
        }

        if (hasClicked) return;
        hasClicked = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        TiltButton(mouseWorldPos);  // 根据鼠标位置进行倾斜
        buttonLight.SetActive(false);  // 关闭按钮灯光
        transform.DOLocalMove(originalPosition - Vector3.up * pressAmount, pressDuration)
            .SetEase(Ease.InQuad)  // 设置加速下沉
            .OnComplete(() =>
            {
                transform.DOLocalMove(originalPosition - Vector3.up * pressAmount * 0.7f, pressDuration);
                // TODO: send reveal cmd
                StartCoroutine(Reveal());
            });
    }

    private IEnumerator Reveal()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
            yield break;
        
        yield return StartCoroutine(ClientEffectExecutor.ValidateActionPoint(ClientGameState.gateway, ClientGameState.playerSlot, 2));
        if (!CommandExecutionState<ValidateActionPointCommand>.Success)
        {
            Debug.Log("没有足够的行动点");
            yield break;
        }

        SpendActionPointCommand apCmd = new SpendActionPointCommand { playerId = ClientGameState.playerSlot, apCount = 2 };
        ClientGameState.gateway.SendCommandServerRpc("SpendActionPoint", JsonConvert.SerializeObject(apCmd), ClientGameState.playerSlot);
        yield return new WaitUntil(() => CommandExecutionState<SpendActionPointCommand>.IsDone);

        ClientCommand.RevealCardsAndScore();
        yield break;
    }

    // 获取鼠标在世界空间的位置
    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return transform.position; // 如果没有击中物体，返回按钮的位置
    }

    // 根据鼠标位置倾斜按钮
    void TiltButton(Vector3 mouseWorldPos)
    {
        Vector3 direction = (mouseWorldPos - pivot.position).normalized;  // 获取按钮与鼠标位置之间的方向

        // 计算X轴和Z轴的角度，限制最大倾斜角度
        float tiltX = Mathf.Clamp(direction.x * maxTiltAngle, -maxTiltAngle, maxTiltAngle);  // 控制X轴的倾斜
        float tiltZ = Mathf.Clamp(direction.z * maxTiltAngle, -maxTiltAngle, maxTiltAngle);  // 控制Z轴的倾斜

        // 应用倾斜，按钮的旋转角度应该根据鼠标方向偏转最大 `maxTiltAngle`
        pivot.DOLocalRotate(originalRotation.eulerAngles + new Vector3(tiltZ, 0, -tiltX), tiltSpeed, RotateMode.Fast);
    }

    // 恢复按钮的倾斜（使其恢复到原始旋转）
    void ResetTilt()
    {
        // 恢复到原来的旋转角度
        pivot.DOLocalRotate(originalRotation.eulerAngles, shakeDuration, RotateMode.Fast);
    }

    // 更新时检查鼠标是否持续悬停在按钮上
    void Update()
    {
        if (isMouseOver)
        {
            // 如果鼠标持续悬停在按钮上，持续触发效果
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            TiltButton(mouseWorldPos);  // 根据鼠标位置持续倾斜按钮
            //transform.DOPunchPosition(Vector3.right * shakeAmount, shakeDuration, 10, 1);  // 持续晃动按钮
        }
    }
}