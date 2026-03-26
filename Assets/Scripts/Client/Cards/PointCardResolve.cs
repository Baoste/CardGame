using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCardResolve : CardInstance
{
    [Header("Component")]
    public TMP_Text pointText;

    public override void InitCardInstance(int cardId, int instanceId)
    {
        base.InitCardInstance(cardId, instanceId);

        localScaleFactor = 0.45f;
        pointText.text = point.ToString();
    }

}
