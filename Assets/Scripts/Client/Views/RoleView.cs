using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoleView : MonoBehaviour
{
    public TextMeshProUGUI roleTMP;

    public void ShowRole(int dealerId)
    {
        if (dealerId == ClientGameState.playerSlot)
        {
            roleTMP.text = "DEALER";
        }
        else
        {
            roleTMP.text = "PLAYER";
        }
    }

    public void ShowWin(int winnerId)
    {
        if (winnerId == ClientGameState.playerSlot)
        {
            roleTMP.text = "WIN!!!";
        }
        else
        {
            roleTMP.text = "LOSE!!!";
        }
    }
}
