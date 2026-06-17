using System;

[Serializable]
public struct AccountData
{
    public long AccountId;
    public string Username;
    public int ChipCount;

    public ChipAppearaceData ChipAppearaceData;

    public AccountData(long accountId, string username, int chipCount, ChipAppearaceData chipAppearaceData)
    {
        AccountId = accountId;
        Username = username;
        ChipCount = chipCount;
        ChipAppearaceData = chipAppearaceData;
    }
}

[Serializable]
public struct ChipAppearaceData
{
    public int ChipColorId;
    public int ChipSkinId;

    public ChipAppearaceData(int chipColorId = 0, int chipSkinId = 0)
    {
        ChipColorId = chipColorId;
        ChipSkinId = chipSkinId;
    }
}

[Serializable]
public struct AccountInfoData
{
    public long AccountId;
    public string SelectColumn;
    public string Value;

    public AccountInfoData(long accountId, string selectColumn, string value)
    {
        AccountId = accountId;
        SelectColumn = selectColumn;
        Value = value;
    }
}

[Serializable]
public struct LeaderboardEntryData
{
    public string Username;
    public int ChipCount;

    public LeaderboardEntryData(string username, int chipCount)
    {
        Username = username;
        ChipCount = chipCount;
    }
}
