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

    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;

    [SerializeField] private TMP_Text messageText;

    [SerializeField] private GameObject chipPrefab;

    private void Awake()
    {
        registerButton.onClick.AddListener(OnClickRegister);
        loginButton.onClick.AddListener(OnClickLogin);

        accountClient.OnRegisterSuccess += OnRegisterSuccess;
        accountClient.OnRegisterFailed += OnRegisterFailed;

        accountClient.OnLoginSuccess += OnLoginSuccess;
        accountClient.OnLoginFailed += OnLoginFailed;
    }

    private void OnDestroy()
    {
        registerButton.onClick.RemoveListener(OnClickRegister);
        loginButton.onClick.RemoveListener(OnClickLogin);

        accountClient.OnRegisterSuccess -= OnRegisterSuccess;
        accountClient.OnRegisterFailed -= OnRegisterFailed;

        accountClient.OnLoginSuccess -= OnLoginSuccess;
        accountClient.OnLoginFailed -= OnLoginFailed;
    }

    private void OnClickRegister()
    {
        string username = usernameInput.text;

        int chipColorId = chipColorDropdown.value;
        int chipSkinId = chipSkinDropdown.value;

        accountClient.Register(username, new ChipAppearaceData(chipColorId, chipSkinId));
        SetMessage("ÕýÔÚ×¢²á...");
    }

    private void OnClickLogin()
    {
        string username = usernameInput.text;

        accountClient.Login(username);
        SetMessage("ÕýÔÚµÇÂ¼...");
    }

    private void OnRegisterSuccess(AccountData account)
    {
        SetMessage(
            $"×¢²á³É¹¦£º{account.Username}£¬³ïÂëÊýÁ¿£º{account.ChipCount}£¬Æ¤·ô£º{account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );

        ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnRegisterFailed(string reason)
    {
        SetMessage($"×¢²áÊ§°Ü£º{reason}");
    }

    private void OnLoginSuccess(AccountData account)
    {
        SetMessage(
            $"µÇÂ¼³É¹¦£º{account.Username}£¬³ïÂëÊýÁ¿£º{account.ChipCount}£¬Æ¤·ô£º{account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );

        ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnLoginFailed(string reason)
    {
        SetMessage($"µÇÂ¼Ê§°Ü£º{reason}");
    }

    private void ApplyChipSkin(ChipAppearaceData chipAppearaceData)
    {
        ChipSkinConfig.Instance.chipAppearaceData = chipAppearaceData;

        // TODO: Test, need to delete
        GameObject obj = Instantiate(chipPrefab);
        ChipViewController chipViewController = obj.GetComponentInChildren<ChipViewController>();
        chipViewController.ChangeMat(ChipSkinConfig.Instance.chipAppearaceData);
        Debug.Log($"[AccountLoginUI] Apply chip skin");
    }

    private void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        Debug.Log(message);
    }
}