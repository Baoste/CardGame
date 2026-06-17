using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using UnityEngine;

public class FishNetAccountServer : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private string databaseFileName = "game.db";

    public static SqliteAccountRepository Repository { get; private set; }

    private readonly Dictionary<NetworkConnection, AccountData> onlineAccounts = new();

    private void Awake()
    {
        if (Repository == null)
        {
            Repository = new SqliteAccountRepository(databaseFileName);
            Repository.Initialize();
        }
    }

    private void OnEnable()
    {
        if (InstanceFinder.ServerManager == null)
        {
            Debug.LogError("[FishNetAccountServer] ServerManager not found.");
            return;
        }

        // If FishNet Authenticator is enabled, register/login requests usually arrive before authentication.
        // Keep requireAuthentication false here so account requests can be received.
        InstanceFinder.ServerManager.RegisterBroadcast<RegisterAccountRequest>(
            OnRegisterAccountRequest,
            requireAuthentication: false
        );

        InstanceFinder.ServerManager.RegisterBroadcast<LoginAccountRequest>(
            OnLoginAccountRequest,
            requireAuthentication: false
        );

        InstanceFinder.ServerManager.RegisterBroadcast<UpdateChipAppearanceRequest>(
            OnUpdateChipAppearanceRequest,
            requireAuthentication: false
        );

        InstanceFinder.ServerManager.RegisterBroadcast<GetAccountInfoRequest>(
            OnGetAccountInfoRequest,
            requireAuthentication: false
        );

        InstanceFinder.ServerManager.RegisterBroadcast<GetLeaderboardRequest>(
            OnGetLeaderboardRequest,
            requireAuthentication: false
        );
    }

    private void OnDisable()
    {
        if (InstanceFinder.ServerManager == null)
            return;

        InstanceFinder.ServerManager.UnregisterBroadcast<RegisterAccountRequest>(
            OnRegisterAccountRequest
        );

        InstanceFinder.ServerManager.UnregisterBroadcast<LoginAccountRequest>(
            OnLoginAccountRequest
        );

        InstanceFinder.ServerManager.UnregisterBroadcast<UpdateChipAppearanceRequest>(
            OnUpdateChipAppearanceRequest
        );

        InstanceFinder.ServerManager.UnregisterBroadcast<GetAccountInfoRequest>(
            OnGetAccountInfoRequest
        );

        InstanceFinder.ServerManager.UnregisterBroadcast<GetLeaderboardRequest>(
            OnGetLeaderboardRequest
        );
    }

    private void OnRegisterAccountRequest(
        NetworkConnection conn,
        RegisterAccountRequest request,
        Channel channel)
    {
        bool success = Repository.TryCreateAccount(
            request.Username,
            request.ChipAppearaceData,
            out AccountData accountData,
            out string errorMessage
        );

        if (success)
        {
            onlineAccounts[conn] = accountData;

            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "Register",
                Success = true,
                Message = "Register success",

                AccountId = accountData.AccountId,
                Username = accountData.Username,
                ChipCount = accountData.ChipCount,
                ChipAppearaceData = accountData.ChipAppearaceData
            });
        }
        else
        {
            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "Register",
                Success = false,
                Message = errorMessage,

                AccountId = 0,
                Username = request.Username,
                ChipCount = 0,
                ChipAppearaceData = request.ChipAppearaceData
            });
        }
    }

    private void OnLoginAccountRequest(
        NetworkConnection conn,
        LoginAccountRequest request,
        Channel channel)
    {
        bool success = Repository.TryLogin(
            request.Username,
            out AccountData accountData,
            out string errorMessage
        );

        if (success)
        {
            onlineAccounts[conn] = accountData;

            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "Login",
                Success = true,
                Message = "Login success",

                AccountId = accountData.AccountId,
                Username = accountData.Username,
                ChipCount = accountData.ChipCount,
                ChipAppearaceData = accountData.ChipAppearaceData
            });
        }
        else
        {
            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "Login",
                Success = false,
                Message = errorMessage,

                AccountId = 0,
                Username = request.Username,
                ChipCount = 0,
                ChipAppearaceData = new ChipAppearaceData()
            });
        }
    }

    private void OnUpdateChipAppearanceRequest(
        NetworkConnection conn,
        UpdateChipAppearanceRequest request,
        Channel channel)
    {
        // Players must log in before updating their own chip appearance.
        if (!onlineAccounts.TryGetValue(conn, out AccountData currentAccount))
        {
            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "UpdateChipAppearance",
                Success = false,
                Message = "Not logged in",

                AccountId = 0,
                Username = string.Empty,
                ChipCount = 0,
                ChipAppearaceData = new ChipAppearaceData()
            });

            return;
        }

        bool success = Repository.TryUpdateChipAppearance(
            currentAccount.AccountId,
            request.ChipAppearaceData,
            out AccountData updatedAccount,
            out string errorMessage
        );

        if (success)
        {
            // Keep the server-side online account cache in sync.
            onlineAccounts[conn] = updatedAccount;

            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "UpdateChipAppearance",
                Success = true,
                Message = "Chip appearance updated",

                AccountId = updatedAccount.AccountId,
                Username = updatedAccount.Username,
                ChipCount = updatedAccount.ChipCount,
                ChipAppearaceData = updatedAccount.ChipAppearaceData
            });
        }
        else
        {
            SendResponse(conn, new AccountResponse
            {
                RequestId = request.RequestId,
                Action = "UpdateChipAppearance",
                Success = false,
                Message = errorMessage,

                AccountId = currentAccount.AccountId,
                Username = currentAccount.Username,
                ChipCount = currentAccount.ChipCount,
                ChipAppearaceData = currentAccount.ChipAppearaceData
            });
        }
    }

    private void OnGetAccountInfoRequest(
        NetworkConnection conn,
        GetAccountInfoRequest request,
        Channel channel)
    {
        bool success = Repository.TryGetAccountInfo(
            request.AccountId,
            request.SelectColumn,
            out AccountInfoData accountInfoData,
            out string errorMessage
        );

        SendInfoResponse(conn, new AccountInfoResponse
        {
            RequestId = request.RequestId,
            Success = success,
            Message = success ? "Query success" : errorMessage,

            AccountId = success ? accountInfoData.AccountId : request.AccountId,
            SelectColumn = success ? accountInfoData.SelectColumn : request.SelectColumn,
            Value = success ? accountInfoData.Value : string.Empty
        });
    }

    private void OnGetLeaderboardRequest(
        NetworkConnection conn,
        GetLeaderboardRequest request,
        Channel channel)
    {
        bool success = Repository.TryGetLeaderboard(
            request.Count,
            out LeaderboardEntryData[] entries,
            out string errorMessage
        );

        SendLeaderboardResponse(conn, new LeaderboardResponse
        {
            RequestId = request.RequestId,
            Success = success,
            Message = success ? "Query success" : errorMessage,
            Entries = success ? entries : new LeaderboardEntryData[0]
        });
    }

    private void SendResponse(NetworkConnection conn, AccountResponse response)
    {
        // The requireAuthenticated argument is false because account requests may arrive before authentication.
        InstanceFinder.ServerManager.Broadcast(
            conn,
            response,
            requireAuthenticated: false,
            channel: Channel.Reliable
        );
    }

    private void SendInfoResponse(NetworkConnection conn, AccountInfoResponse response)
    {
        InstanceFinder.ServerManager.Broadcast(
            conn,
            response,
            requireAuthenticated: false,
            channel: Channel.Reliable
        );
    }

    private void SendLeaderboardResponse(NetworkConnection conn, LeaderboardResponse response)
    {
        InstanceFinder.ServerManager.Broadcast(
            conn,
            response,
            requireAuthenticated: false,
            channel: Channel.Reliable
        );
    }

    public bool TryGetOnlineAccount(NetworkConnection conn, out AccountData accountData)
    {
        return onlineAccounts.TryGetValue(conn, out accountData);
    }

    public void RemoveOnlineAccount(NetworkConnection conn)
    {
        onlineAccounts.Remove(conn);
    }
}
