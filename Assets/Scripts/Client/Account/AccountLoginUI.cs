using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishNetAccountClient accountClient;

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Dropdown chipColorDropdown;
    [SerializeField] private TMP_Dropdown chipSkinDropdown;

    [Header("Single Button")]
    [SerializeField] private Button enterButton;

    [Header("Test")]
    [SerializeField] private GameObject chipPrefab;

    private string pendingUsername;
    private ChipAppearaceData pendingChipAppearaceData;

    private bool isWaitingResponse;

    private void Awake()
    {
        enterButton.onClick.AddListener(OnClickEnter);

        accountClient.OnRegisterSuccess += OnRegisterSuccess;
        accountClient.OnRegisterFailed += OnRegisterFailed;

        accountClient.OnLoginSuccess += OnLoginSuccess;
        accountClient.OnLoginFailed += OnLoginFailed;
    }

    private void OnDestroy()
    {
        enterButton.onClick.RemoveListener(OnClickEnter);

        accountClient.OnRegisterSuccess -= OnRegisterSuccess;
        accountClient.OnRegisterFailed -= OnRegisterFailed;

        accountClient.OnLoginSuccess -= OnLoginSuccess;
        accountClient.OnLoginFailed -= OnLoginFailed;
    }

    private void OnClickEnter()
    {
        if (isWaitingResponse)
            return;

        pendingUsername = usernameInput.text;

        int chipColorId = chipColorDropdown.value;
        int chipSkinId = chipSkinDropdown.value;

        pendingChipAppearaceData = new ChipAppearaceData(chipColorId, chipSkinId);

        isWaitingResponse = true;
        enterButton.interactable = false;

        // 先尝试登录
        accountClient.Login(pendingUsername);

        SetMessage("正在登录...");
    }

    private void OnLoginSuccess(AccountData account)
    {
        FinishRequest();

        SetMessage(
            $"登录成功：{account.Username}，筹码数量：{account.ChipCount}，皮肤：{account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );

        ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnLoginFailed(string reason)
    {
        // 如果服务器返回账号不存在，就自动注册
        if (reason == "账号不存在")
        {
            SetMessage("账号不存在，正在自动注册...");

            accountClient.Register(pendingUsername, pendingChipAppearaceData);
            return;
        }

        FinishRequest();
        SetMessage($"登录失败：{reason}");
    }

    private void OnRegisterSuccess(AccountData account)
    {
        FinishRequest();

        SetMessage(
            $"注册成功：{account.Username}，筹码数量：{account.ChipCount}，皮肤：{account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );

        ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnRegisterFailed(string reason)
    {
        FinishRequest();
        SetMessage($"注册失败：{reason}");
    }

    private void FinishRequest()
    {
        isWaitingResponse = false;

        if (enterButton != null)
            enterButton.interactable = true;
    }

    private void ApplyChipSkin(ChipAppearaceData chipAppearaceData)
    {
        ChipSkinConfig.Instance.myChipAppearaceData = chipAppearaceData;

        // TODO: Test, need to delete
        GameObject obj = Instantiate(chipPrefab);
        ChipViewController chipViewController = obj.GetComponentInChildren<ChipViewController>();
        chipViewController.ChangeMat(ChipSkinConfig.Instance.myChipAppearaceData);
        Debug.Log("[AccountLoginUI] Apply chip skin");
    }

    private void SetMessage(string message)
    {
        Debug.Log(message);
    }
}