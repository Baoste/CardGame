using DG.Tweening;
using Game.Domain;
using GameKit.Dependencies.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDrawSkillCard : MonoBehaviour, IMouseClick, IMouseEnter, IMouseExit, IMouseStay
{
    [SerializeField] private Light pointLight;
    [SerializeField] private Transform skillCard;

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
            StartCoroutine(DrawSkillCard());
        }
    }

    public void MouseEnter()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingSkillCard) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId) return;

        pointLight.color = Color.green;
        skillCard.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(skillCard.DOLocalMoveX(-0.00493f, 0.2f));
    }

    public void MouseStay()
    {
        if (ClientEffectContext.isExecutingSkillCard ||
            ClientEffectContext.isDrawingSkillCard ||
            ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId
        )
        {
            skillCard.localPosition = new Vector3(-0.003446764f, skillCard.localPosition.y, skillCard.localPosition.z);
        }
    }

    public void MouseExit()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingSkillCard) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId) return;

        pointLight.color = Color.white;
        skillCard.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(skillCard.DOLocalMoveX(-0.003446764f, 0.2f));
    }

    private IEnumerator DrawSkillCard()
    {
        DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawSkillCard", JsonConvert.SerializeObject(cmd));
        yield break;
    }
}
