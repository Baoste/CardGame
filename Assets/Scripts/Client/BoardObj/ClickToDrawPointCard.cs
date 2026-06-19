using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDrawPointCard : MonoBehaviour, IMouseClick, IMouseEnter, IMouseExit, IMouseStay
{
    [SerializeField] private Transform disk;
    [SerializeField] private Renderer screenRenderer;
    private bool diskIsOut = true;
    private bool screenLight = false;

    private void Update()
    {
        if (!screenLight && ClientGameState.Instance.CurrentPlayerId != -1 && ClientGameState.Instance.CurrentPlayerId == ClientGameState.playerSlot && ClientEffectContext.Instance.drawPointCardCount == 0)
        {
            screenRenderer.material.SetFloat("_LightController", 1);
            screenLight = true;
        }
    }

    public void MouseClick()
    {
        // 教程特定
        if (ClientGameState.IsTutorial)
        {
            ClientGameState.TutorialStepDone = true;
            return;
        }

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
            screenRenderer.material.SetFloat("_LightController", 0);
            screenLight = false;
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
