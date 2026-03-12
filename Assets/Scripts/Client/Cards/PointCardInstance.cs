using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCardInstance : CardInstance
{
    [Header("Component")]
    public TMP_Text pointText;

    public void Awake()
    {
    }

    public override void InitCardInstance(int cardId, int instaceId)
    {
        base.InitCardInstance(cardId, instaceId);

        pointText.text = point.ToString();
    }
}
