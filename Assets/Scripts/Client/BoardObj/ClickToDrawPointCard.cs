using DG.Tweening;
using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDrawPointCard : MonoBehaviour, IMouseClick, IMouseEnter, IMouseExit, IMouseStay
{
    [SerializeField] private Transform disk;
    private bool diskIsOut = true;

    public void MouseClick()
    {
        if (ClientEffectContext.isExecutingSkillCard) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            Debug.Log("不是你的回合");
            return;
        }

        if (ClientGameState.Instance.CurrentPlayerId != -1 && ClientGameState.Instance.CurrentPlayerId == ClientGameState.playerSlot)
        {
            StartCoroutine(DrawPointCard());
            ClientEffectContext.Instance.drawPointCardCount++;
        }
    }

    private IEnumerator DrawPointCard()
    {
        DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));

        yield break;
    }

    public void MouseEnter()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingPointCard || ClientEffectContext.Instance.drawPointCardCount > 0) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId) return;
        
        diskIsOut = true;
        disk.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(disk.DOLocalMoveZ(-0.18f, 0.2f));
    }

    public void MouseStay()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingPointCard || ClientEffectContext.Instance.drawPointCardCount > 0 ||
            ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            diskIsOut = false;
            disk.localPosition = new Vector3(disk.localPosition.x, disk.localPosition.y, 0f);
        }
    }

    public void MouseExit()
    {
        if (diskIsOut)
        {
            diskIsOut = false;
            disk.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.Append(disk.DOLocalMoveZ(0f, 0.2f));
        }
    }
}
