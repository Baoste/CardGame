using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCardResolve : CardInstance
{
    public CardVisualState cardVisualState { get; private set; }
    private PointCardViewController viewController;

    private void Awake()
    {
        viewController = GetComponent<PointCardViewController>();
    }

    public override void InitCardInstance(int cardId, int instanceId)
    {
        base.InitCardInstance(cardId, instanceId);

        localScaleFactor = 0.45f;
        viewController.SetCardTexture_None(point);
    }

    public void InitCardState(CardVisualState cardVisualState)
    {
        this.cardVisualState = cardVisualState;

        switch (cardVisualState)
        {
            case CardVisualState.None:
                break;
            case CardVisualState.Hidden:
                viewController.SetCardTexture_Hidden();
                // TODO: change mat
                break;
            case CardVisualState.Locked:
                // TODO: change mat
                break;
        }
    }
}
