using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance : MonoBehaviour
{
    [Header("Value")]
    public int instanceId;
    public int point;
    public float localScaleFactor;

    public virtual void InitCardInstance(int cardId, int _instanceId)
    {
        instanceId = _instanceId;
        point = CardDatabase.Get(cardId).point;
        localScaleFactor = 0.4f;
    }

}
