using System;
using FishNet;
using FishNet.Transporting;
using UnityEngine;

public class FishNetAccountClient : MonoBehaviour
{
    public event Action<AccountData> OnRegisterSuccess;
    public event Action<string> OnRegisterFailed;

    public event Action<AccountData> OnLoginSuccess;
    public event Action<string> OnLoginFailed;

    public event Action<AccountData> OnUpdateChipAppearanceSuccess;
    public event Action<string> OnUpdateChipAppearanceFailed;

    public event Action<AccountInfoData> OnGetAccountInfoSuccess;
    public event Action<string> OnGetAccountInfoFailed;

    public event Action<LeaderboardEntryData[]> OnGetLeaderboardSuccess;
    public event Action<string> OnGetLeaderboardFailed;

    public AccountData CurrentAccount { get; private set; }
    public bool HasAccount { get; private set; }

    private int nextRequestId = 1;

    private void OnEnable()
    {
        if (InstanceFinder.ClientManager == null)
        {
            Debug.LogError("[FishNetAccountClient] ClientManager not found.");
            return;
        }

        InstanceFinder.ClientManager.RegisterBroadcast<AccountResponse>(
            OnAccountResponse
        );

        InstanceFinder.ClientManager.RegisterBroadcast<AccountInfoResponse>(
            OnAccountInfoResponse
        );

        InstanceFinder.ClientManager.RegisterBroadcast<LeaderboardResponse>(
            OnLeaderboardResponse
        );
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager == null)
            return;

        InstanceFinder.ClientManager.UnregisterBroadcast<AccountResponse>(
            OnAccountResponse
        );

        InstanceFinder.ClientManager.UnregisterBroadcast<AccountInfoResponse>(
            OnAccountInfoResponse
        );

        InstanceFinder.ClientManager.UnregisterBroadcast<LeaderboardResponse>(
            OnLeaderboardResponse
        );
    }

    public void Register(string username, ChipAppearaceData chipAppearaceData)
    {
        if (!CanSendRequest())
            return;

        RegisterAccountRequest request = new RegisterAccountRequest
        {
            RequestId = nextRequestId++,
            Username = username,
            ChipAppearaceData = chipAppearaceData
        };

        InstanceFinder.ClientManager.Broadcast(
            request,
            Channel.Reliable
        );
    }

    public void GetLeaderboard(int count)
    {
        if (!CanSendRequest())
            return;

        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            RequestId = nextRequestId++,
            Count = count
        };

        InstanceFinder.ClientManager.Broadcast(
            request,
            Channel.Reliable
        );
    }

    public void Login(string username)
    {
        if (!CanSendRequest())
            return;

        LoginAccountRequest request = new LoginAccountRequest
        {
            RequestId = nextRequestId++,
            Username = username
        };

        InstanceFinder.ClientManager.Broadcast(
            request,
            Channel.Reliable
        );
    }

    public void UpdateChipAppearance(ChipAppearaceData chipAppearaceData)
    {
        if (!CanSendRequest())
            return;

        UpdateChipAppearanceRequest request = new UpdateChipAppearanceRequest
        {
            RequestId = nextRequestId++,
            ChipAppearaceData = chipAppearaceData
        };

        InstanceFinder.ClientManager.Broadcast(
            request,
            Channel.Reliable
        );
    }

    public void GetAccountInfo(string selectColumn, long accountId)
    {
        if (!CanSendRequest())
            return;

        GetAccountInfoRequest request = new GetAccountInfoRequest
        {
            RequestId = nextRequestId++,
            AccountId = accountId,
            SelectColumn = selectColumn
        };

        InstanceFinder.ClientManager.Broadcast(
            request,
            Channel.Reliable
        );
    }

    private void OnAccountResponse(AccountResponse response, Channel channel)
    {
        if (response.Success)
        {
            CurrentAccount = new AccountData(
                response.AccountId,
                response.Username,
                response.ChipCount,
                response.ChipAppearaceData
            );

            HasAccount = true;

            ChipSkinConfig.myAccountData = CurrentAccount;

            if (response.Action == "Register")
            {
                OnRegisterSuccess?.Invoke(CurrentAccount);
            }
            else if (response.Action == "Login")
            {
                OnLoginSuccess?.Invoke(CurrentAccount);
            }
            else if (response.Action == "UpdateChipAppearance")
            {
                OnUpdateChipAppearanceSuccess?.Invoke(CurrentAccount);
            }

            Debug.Log(
                $"[AccountClient] {response.Action} success. " +
                $"AccountId={CurrentAccount.AccountId}, " +
                $"Username={CurrentAccount.Username}, " +
                $"ChipCount={CurrentAccount.ChipCount}, " +
                $"ChipColorId={CurrentAccount.ChipAppearaceData.ChipColorId}, " +
                $"ChipSkinId={CurrentAccount.ChipAppearaceData.ChipSkinId}"
            );
        }
        else
        {
            if (response.Action == "Register")
            {
                OnRegisterFailed?.Invoke(response.Message);
            }
            else if (response.Action == "Login")
            {
                OnLoginFailed?.Invoke(response.Message);
            }
            else if (response.Action == "UpdateChipAppearance")
            {
                OnUpdateChipAppearanceFailed?.Invoke(response.Message);
            }

            Debug.LogWarning(
                $"[AccountClient] {response.Action} failed: {response.Message}"
            );
        }
    }

    private void OnAccountInfoResponse(AccountInfoResponse response, Channel channel)
    {
        if (response.Success)
        {
            AccountInfoData accountInfoData = new AccountInfoData(
                response.AccountId,
                response.SelectColumn,
                response.Value
            );

            OnGetAccountInfoSuccess?.Invoke(accountInfoData);

            Debug.Log(
                $"[AccountClient] GetAccountInfo success. " +
                $"AccountId={accountInfoData.AccountId}, " +
                $"SelectColumn={accountInfoData.SelectColumn}, " +
                $"Value={accountInfoData.Value}"
            );
        }
        else
        {
            OnGetAccountInfoFailed?.Invoke(response.Message);

            Debug.LogWarning(
                $"[AccountClient] GetAccountInfo failed: {response.Message}"
            );
        }
    }

    private void OnLeaderboardResponse(LeaderboardResponse response, Channel channel)
    {
        if (response.Success)
        {
            LeaderboardEntryData[] entries = response.Entries ?? Array.Empty<LeaderboardEntryData>();

            OnGetLeaderboardSuccess?.Invoke(entries);

            Debug.Log(
                $"[AccountClient] GetLeaderboard success. Count={entries.Length}"
            );
        }
        else
        {
            OnGetLeaderboardFailed?.Invoke(response.Message);

            Debug.LogWarning(
                $"[AccountClient] GetLeaderboard failed: {response.Message}"
            );
        }
    }

    private bool CanSendRequest()
    {
        if (InstanceFinder.ClientManager == null)
        {
            Debug.LogError("[AccountClient] ClientManager is null.");
            return false;
        }

        if (!InstanceFinder.ClientManager.Started)
        {
            Debug.LogError("[AccountClient] Client is not connected to server.");
            return false;
        }

        return true;
    }
}
