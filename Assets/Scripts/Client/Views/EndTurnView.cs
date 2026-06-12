using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnView : MonoBehaviour, IViewClear, IMouseDown
{
    public Light btnLight;
    public bool hasClicked = true;

    private float clickDist = 0.05f;

    public void ClearView()
    {
        hasClicked = true;
        btnLight.intensity = 0;
    }

    public void MouseDown()
    {
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId || hasClicked)
            return;
       
        transform.DOMoveY(transform.position.y - clickDist, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AudioManager.Instance.Play("EndTurn");

            hasClicked = true;
            btnLight.intensity = 0;
            ClientCommand.EndTurn();
            transform.DOMoveY(transform.position.y + clickDist, 0.1f).SetEase(Ease.OutQuad);
        });
    }
}
