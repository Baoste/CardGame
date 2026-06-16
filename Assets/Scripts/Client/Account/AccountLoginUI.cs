using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishNetAccountClient accountClient;

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private UIModelPreview UIModelPreview;

    [Header("Single Button")]
    [SerializeField] private Button updateConfirmButton;
    [SerializeField] private MainMenuUI mainMenuUI;

    //[Header("Test")]
    //[SerializeField] private GameObject chipPrefab;

    private string pendingUsername;
    private ChipAppearaceData pendingChipAppearaceData;

    private bool isWaitingResponse;

    private void Awake()
    {
        updateConfirmButton.onClick.AddListener(OnClickUpdateChip);

        accountClient.OnRegisterSuccess += OnRegisterSuccess;
        accountClient.OnRegisterFailed += OnRegisterFailed;

        accountClient.OnLoginSuccess += OnLoginSuccess;
        accountClient.OnLoginFailed += OnLoginFailed;

        accountClient.OnUpdateChipAppearanceSuccess += OnUpdateChipAppearanceSuccess;
        accountClient.OnUpdateChipAppearanceFailed += OnUpdateChipAppearanceFailed;
    }

    private void Update()
    {
        if (mainMenuUI.loginCanvas.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            OnClickEnter();
        }
    }

    private void OnDestroy()
    {
        accountClient.OnRegisterSuccess -= OnRegisterSuccess;
        accountClient.OnRegisterFailed -= OnRegisterFailed;

        accountClient.OnLoginSuccess -= OnLoginSuccess;
        accountClient.OnLoginFailed -= OnLoginFailed;

        accountClient.OnUpdateChipAppearanceSuccess -= OnUpdateChipAppearanceSuccess;
        accountClient.OnUpdateChipAppearanceFailed -= OnUpdateChipAppearanceFailed;
    }

    private void OnClickEnter()
    {
        if (isWaitingResponse)
            return;

        pendingUsername = usernameInput.text;

        pendingChipAppearaceData = new ChipAppearaceData(0, 0);

        isWaitingResponse = true;

        // Try login first.
        accountClient.Login(pendingUsername);

        SetMessage("Logging in...");
    }

    private void OnClickUpdateChip()
    {
        if (isWaitingResponse)
            return;

        pendingChipAppearaceData = UIModelPreview.ChipAppearaceData;

        isWaitingResponse = true;

        accountClient.UpdateChipAppearance(pendingChipAppearaceData);

        SetMessage("Updating chip appearance...");
    }

    private void OnLoginSuccess(AccountData account)
    {
        FinishRequest();

        SetMessage(
            $"Login success: {account.Username}, chips: {account.ChipCount}, skin: {account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );
        mainMenuUI.ShowMenu();
        // ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnLoginFailed(string reason)
    {
        // Auto-register if the account does not exist.
        if (reason == "Account not found")
        {
            SetMessage("Account not found, auto-registering...");

            accountClient.Register(pendingUsername, pendingChipAppearaceData);
            return;
        }

        FinishRequest();
        SetMessage($"Login failed: {reason}");
    }

    private void OnRegisterSuccess(AccountData account)
    {
        FinishRequest();

        SetMessage(
            $"Register success: {account.Username}, chips: {account.ChipCount}, skin: {account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );
        mainMenuUI.ShowClipUpdate();
        // ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnRegisterFailed(string reason)
    {
        FinishRequest();
        SetMessage($"Register failed: {reason}");
    }

    private void OnUpdateChipAppearanceSuccess(AccountData account)
    {
        FinishRequest();

        SetMessage(
            $"Chip appearance updated: {account.ChipAppearaceData.ChipColorId} | {account.ChipAppearaceData.ChipSkinId}"
        );

        // ApplyChipSkin(account.ChipAppearaceData);
    }

    private void OnUpdateChipAppearanceFailed(string reason)
    {
        FinishRequest();

        SetMessage($"Chip appearance update failed: {reason}");
    }

    private void FinishRequest()
    {
        isWaitingResponse = false;
    }

    private void ApplyChipSkin(ChipAppearaceData chipAppearaceData)
    {
        // FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("gxz");

        // TODO: Test, need to delete
        //GameObject obj = Instantiate(chipPrefab);
        //ChipViewController chipViewController = obj.GetComponentInChildren<ChipViewController>();
        //chipViewController.ChangeMat(ChipSkinConfig.Instance.myAccountData.ChipAppearaceData);
        Debug.Log("[AccountLoginUI] Apply chip skin");
    }

    private void SetMessage(string message)
    {
        Debug.Log(message);
    }
}
