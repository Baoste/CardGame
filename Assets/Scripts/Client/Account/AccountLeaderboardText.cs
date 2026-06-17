using System.Text;
using TMPro;
using UnityEngine;

public class AccountLeaderboardText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishNetAccountClient accountClient;
    [SerializeField] private TMP_Text leaderboardText;

    [Header("Leaderboard")]
    [SerializeField] private int count = 10;

    [Header("Text")]
    [SerializeField] private string loadingText = "...";
    [SerializeField] private string emptyText = "No leaderboard data";
    [SerializeField] private string failedText = "--";

    private bool isWaitingForLeaderboard;

    private void OnEnable()
    {
        if (accountClient != null)
        {
            accountClient.OnGetLeaderboardSuccess += OnGetLeaderboardSuccess;
            accountClient.OnGetLeaderboardFailed += OnGetLeaderboardFailed;
        }
    }

    private void OnDisable()
    {
        if (accountClient == null)
            return;

        accountClient.OnGetLeaderboardSuccess -= OnGetLeaderboardSuccess;
        accountClient.OnGetLeaderboardFailed -= OnGetLeaderboardFailed;
    }

    public void RefreshLeaderboard()
    {
        if (leaderboardText != null)
            leaderboardText.text = loadingText;

        if (accountClient == null)
        {
            Debug.LogError("[AccountLeaderboardText] FishNetAccountClient not found.");
            SetFailedText();
            return;
        }

        isWaitingForLeaderboard = true;
        accountClient.GetLeaderboard(count);
    }

    private void OnGetLeaderboardSuccess(LeaderboardEntryData[] entries)
    {
        if (!isWaitingForLeaderboard)
            return;

        isWaitingForLeaderboard = false;

        if (leaderboardText == null)
            return;

        if (entries == null || entries.Length <= 0)
        {
            leaderboardText.text = emptyText;
            return;
        }

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < entries.Length; i++)
        {
            LeaderboardEntryData entry = entries[i];
            builder.Append(i + 1);
            builder.Append(". ");
            builder.Append(entry.Username);
            builder.Append("  ");
            builder.Append(entry.ChipCount);

            if (i < entries.Length - 1)
                builder.AppendLine();
        }

        leaderboardText.text = builder.ToString();
    }

    private void OnGetLeaderboardFailed(string reason)
    {
        if (!isWaitingForLeaderboard)
            return;

        isWaitingForLeaderboard = false;
        SetFailedText();

        Debug.LogWarning("[AccountLeaderboardText] Get leaderboard failed: " + reason);
    }

    private void SetFailedText()
    {
        if (leaderboardText != null)
            leaderboardText.text = failedText;
    }
}
