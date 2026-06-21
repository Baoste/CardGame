using System.Text;
using TMPro;
using UnityEngine;

public class AccountLeaderboardText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishNetAccountClient accountClient;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text userText;
    [SerializeField] private TMP_Text chipsText;

    [Header("Leaderboard")]
    [SerializeField] private int count = 10;

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
        if (accountClient == null)
        {
            Debug.LogError("[AccountLeaderboardText] FishNetAccountClient not found.");
            // SetFailedText();
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

        if (entries == null || entries.Length == 0)
        {
            rankText.text = string.Empty;
            userText.text = string.Empty;
            chipsText.text = string.Empty;
            return;
        }

        StringBuilder rankBuilder = new StringBuilder();
        StringBuilder userBuilder = new StringBuilder();
        StringBuilder chipsBuilder = new StringBuilder();

        for (int i = 0; i < entries.Length; i++)
        {
            LeaderboardEntryData entry = entries[i];

            string username = entry.Username ?? string.Empty;
            if (username.Length > 13)
                username = username.Substring(0, 13);

            rankBuilder.Append(i + 1);
            userBuilder.Append(username);
            chipsBuilder.Append(entry.ChipCount);

            if (i < entries.Length - 1)
            {
                rankBuilder.AppendLine();
                userBuilder.AppendLine();
                chipsBuilder.AppendLine();
            }
        }

        rankText.text = rankBuilder.ToString();
        userText.text = userBuilder.ToString();
        chipsText.text = chipsBuilder.ToString();
    }

    private void OnGetLeaderboardFailed(string reason)
    {
        if (!isWaitingForLeaderboard)
            return;

        isWaitingForLeaderboard = false;
        // SetFailedText();

        Debug.LogWarning("[AccountLeaderboardText] Get leaderboard failed: " + reason);
    }

    //private void SetFailedText()
    //{
    //    if (leaderboardText != null)
    //        leaderboardText.text = failedText;
    //}
}
