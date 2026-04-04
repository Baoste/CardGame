using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnView : MonoBehaviour, IMouseDown
{
    public Light btnLight;

    private float clickDist = 0.05f;

    public void MouseDown()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
            return;
       
        transform.DOMoveY(transform.position.y - clickDist, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            btnLight.intensity = 0;
            ClientCommand.EndTurn();
            ClientCommand.StartTurn(1 - ClientGameState.playerSlot);
            transform.DOMoveY(transform.position.y + clickDist, 0.1f).SetEase(Ease.OutQuad);
        });
    }
}
