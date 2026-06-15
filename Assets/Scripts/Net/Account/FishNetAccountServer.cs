using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using UnityEngine;

public class FishNetAccountServer : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private string databaseFileName = "game.db";

    private SqliteAccountRepository repository;

    private readonly Dictionary<NetworkConnection, AccountData> onlineAccounts = new();

    private void Awake()
    {
        repository = new SqliteAccountRepository(databaseFileName);
        repository.Initialize();
    }

    private void OnEnable()
    {
        if (InstanceFinder.ServerManager == null)
        {
            Debug.LogError("[FishNetAccountServer] ServerManager not found.");
            return;
        }

        // 如果你启用了 FishNet Authenticator，注册/登录阶段通常还未认证，
        // 所以这里 requireAuthentication 要传 false。
        InstanceFinder.ServerManager.RegisterBroadcast<RegisterAccountRequest>(
            OnRegisterAccountRequest,
            requireAuthentication: false
        );

        InstanceFinder.ServerManager.RegisterBroadcast<LoginAccountRequest>(
            OnLoginAccountRequest,
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
    }

    private void OnRegisterAccountRequest(
        NetworkConnection conn,
        RegisterAccountRequest request,
        Channel channel)
    {
        bool success = repository.TryCreateAccount(
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
                Message = "注册成功",

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
        bool success = repository.TryLogin(
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
                Message = "登录成功",

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

    private void SendResponse(NetworkConnection conn, AccountResponse response)
    {
        // 第三个参数 requireAuthenticated = false。
        // 因为注册/登录阶段客户端可能还没通过 Authenticator。
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