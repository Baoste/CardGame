using DG.Tweening;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickToDrawSkillCard : MonoBehaviour, IMouseClick, IMouseEnter, IMouseExit, IMouseStay
{
    [SerializeField] private Transform skillCard;
    [SerializeField] private Material buttomMat;
    [SerializeField] private TMP_Text skillCardCountText;
    public Stack<GameObject> SkillCardStack;

    [SerializeField] private string[] EmojiStrings;
    private string countEmoji;
    private Coroutine animCoroutine;

    private void Start()
    {
        countEmoji = EmojiStrings[0];
        skillCardCountText.text = countEmoji;
        SkillCardStack = new Stack<GameObject>();
        ResetCardStack();
    }

    public void Draw1Card()
    {
        ChangeCountStr(ClientGameState.SkillCardCount);

        if (ClientGameState.SkillCardCount < 6 && SkillCardStack.Count > 0)
        {
            GameObject topCard = SkillCardStack.Pop();
            topCard.SetActive(false);
        }
        // 当牌堆中有4张牌时，触发遮罩动画
        if (ClientGameState.SkillCardCount == 4)
        {
            DOTween.To(
                () => buttomMat.GetFloat("_MaskAppearProgress"),
                x => buttomMat.SetFloat("_MaskAppearProgress", x),
                2f, // 目标值
                3f  // 持续时间
            ).SetEase(Ease.Linear);
        }
    }

    private void ChangeCountStr(int count)
    {
        string str = "";
        if (count < 4)
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            animCoroutine = StartCoroutine(PlayStrAnim("( /owo)|(  /ow)|(   /o)|(    /)|(     )|(\\    )|(o\\   )|(wo\\  )|(owo\\ )|(/owo\\)"));
        }
        else if (count < 7)
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            animCoroutine = StartCoroutine(PlayStrAnim("( '.ω.)|(  '.ω)|(   '.)|(    ')|(     )|(     )|('    )|(.'   )|(ω.' )|(.ω.' )|('.ω.')"));
        }
        else
        {
            str = EmojiStrings[Random.Range(0, EmojiStrings.Length)];
            countEmoji = str;
            skillCardCountText.text = countEmoji;
        }
    }

    private IEnumerator PlayStrAnim(string str)
    {
        yield return new WaitForSeconds(0.5f);

        string[] parts = str.Split('|');
        foreach (string part in parts)
        {
            countEmoji = part;
            skillCardCountText.text = countEmoji;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ResetCardStack()
    {
        buttomMat.SetFloat("_MaskAppearProgress", -1f);
        SkillCardStack.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(true);
            SkillCardStack.Push(child);
        }
    }

    public void MouseClick()
    {
        // 教程特定
        if (ClientGameState.IsTutorial)
        {
            skillCard.DOLocalMoveX(-0.003446764f, 0.2f);
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
            StartCoroutine(DrawSkillCard());
        }
    }

    public void MouseEnter()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingSkillCard || ClientGameState.SkillCardCount < 1 ||
            ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            return;
        }

        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        skillCardCountText.text = ClientGameState.SkillCardCount.ToString();

        skillCard.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(skillCard.DOLocalMoveX(-0.00493f, 0.2f));
    }

    public void MouseStay()
    {
        if (ClientGameState.SkillCardCount < 1) return;

        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingSkillCard ||
            ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
        {
            skillCard.localPosition = new Vector3(-0.003446764f, skillCard.localPosition.y, skillCard.localPosition.z);
        }
    }

    public void MouseExit()
    {
        if (ClientEffectContext.isExecutingSkillCard || ClientEffectContext.isDrawingSkillCard || ClientGameState.SkillCardCount < 1) return;
        if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId) return;

        skillCardCountText.text = countEmoji;
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
