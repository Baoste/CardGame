using FishNet.Broadcast;

public struct RegisterAccountRequest : IBroadcast
{
    public int RequestId;
    public string Username;

    public ChipAppearaceData ChipAppearaceData;
}

public struct LoginAccountRequest : IBroadcast
{
    public int RequestId;
    public string Username;
}

public struct UpdateChipAppearanceRequest : IBroadcast
{
    public int RequestId;
    public ChipAppearaceData ChipAppearaceData;
}

public struct AccountResponse : IBroadcast
{
    public int RequestId;

    // "Register" / "Login"
    public string Action;

    public bool Success;
    public string Message;

    public long AccountId;
    public string Username;
    public int ChipCount;

    public ChipAppearaceData ChipAppearaceData;
}