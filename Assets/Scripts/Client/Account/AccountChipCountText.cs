using TMPro;
using UnityEngine;

public class AccountChipCountText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishNetAccountClient accountClient;
    [SerializeField] private TMP_Text chipCountText;
    [SerializeField] private SlotNumberTMP slotNumberTMP;

    [Header("Text")]
    [SerializeField] private string loadingText = "...";
    [SerializeField] private string failedText = "--";

    private long requestedAccountId;
    private bool isWaitingForChipCount;

    private void OnEnable()
    {
        if (accountClient == null)
            return;

        accountClient.OnGetAccountInfoSuccess += OnGetAccountInfoSuccess;
        accountClient.OnGetAccountInfoFailed += OnGetAccountInfoFailed;
    }

    private void OnDisable()
    {
        if (accountClient == null)
            return;

        accountClient.OnGetAccountInfoSuccess -= OnGetAccountInfoSuccess;
        accountClient.OnGetAccountInfoFailed -= OnGetAccountInfoFailed;
    }

    public void RefreshChipCount()
    {
        if (chipCountText != null)
            chipCountText.text = loadingText;

        if (accountClient == null)
        {
            Debug.LogError("[AccountChipCountText] FishNetAccountClient not found.");
            SetFailedText();
            return;
        }

        if (!TryGetAccountId(out requestedAccountId))
        {
            Debug.LogWarning("[AccountChipCountText] AccountId not found.");
            SetFailedText();
            return;
        }

        isWaitingForChipCount = true;
        accountClient.GetAccountInfo("chip_count", requestedAccountId);
    }

    private void OnGetAccountInfoSuccess(AccountInfoData accountInfoData)
    {
        if (!isWaitingForChipCount)
            return;

        if (accountInfoData.AccountId != requestedAccountId)
            return;

        if (accountInfoData.SelectColumn != "chip_count")
            return;

        isWaitingForChipCount = false;

        if (chipCountText != null)
        {
            int chipCount = int.Parse(accountInfoData.Value);
            chipCountText.text = accountInfoData.Value;
            slotNumberTMP.RollTo(chipCount);
            ChipSkinConfig.myAccountData.ChipCount = chipCount;
        }
    }

    private void OnGetAccountInfoFailed(string reason)
    {
        if (!isWaitingForChipCount)
            return;

        isWaitingForChipCount = false;
        SetFailedText();

        Debug.LogWarning("[AccountChipCountText] Get chip_count failed: " + reason);
    }

    private bool TryGetAccountId(out long accountId)
    {
        if (accountClient != null && accountClient.HasAccount)
        {
            accountId = accountClient.CurrentAccount.AccountId;
            return accountId > 0;
        }

        accountId = ChipSkinConfig.myAccountData.AccountId;
        return accountId > 0;
    }

    private void SetFailedText()
    {
        if (chipCountText != null)
            chipCountText.text = failedText;
    }
}
