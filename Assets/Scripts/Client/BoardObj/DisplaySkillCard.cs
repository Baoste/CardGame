using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySkillCard : MonoBehaviour
{
    [SerializeField] private SkillCardInstance skillCard;

    private readonly Queue<SkillCardData> displayQueue = new();
    private bool isDisplaying;
    private Sequence currentSeq;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
    }

    private struct SkillCardData
    {
        public string CardName;
        public string Description;
        public int Point;

        public SkillCardData(string cardName, string description, int point)
        {
            CardName = cardName;
            Description = description;
            Point = point;
        }
    }

    public void Display(string cardName, string description, int point)
    {
        displayQueue.Enqueue(new SkillCardData(cardName, description, point));

        if (!isDisplaying)
        {
            PlayNext();
        }
    }

    private void PlayNext()
    {
        if (displayQueue.Count <= 0)
        {
            isDisplaying = false;
            return;
        }

        isDisplaying = true;

        SkillCardData data = displayQueue.Dequeue();

        skillCard.nameText.text = data.CardName;
        skillCard.descriptionText.text = data.Description;
        skillCard.pointText.text = data.Point.ToString();

        // 初始状态：缩小 + 正面
        transform.localScale = Vector3.zero;

        currentSeq?.Kill();

        currentSeq = DOTween.Sequence();

        // 展示：放大 + 翻到背面/正面，看你的卡牌朝向
        currentSeq.Append(
            transform.DORotate(
                new Vector3(0f, 180f, 0f),
                0.5f,
                RotateMode.LocalAxisAdd
            )
        );

        currentSeq.Join(
            transform.DOScale(Vector3.one, 0.5f)
        );

        currentSeq.AppendInterval(1.5f);

        // 收起：再翻 180 + 缩小
        currentSeq.Append(
            transform.DORotate(
                new Vector3(0f, 180f, 0f),
                0.5f,
                RotateMode.LocalAxisAdd
            )
        );

        currentSeq.Join(
            transform.DOScale(Vector3.zero, 0.5f)
        );

        currentSeq.OnComplete(PlayNext);
    }
}