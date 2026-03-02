using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFunctions : MonoBehaviour
{
    [ContextMenu("Test SkillCard To File")]
    public void Test0()
    {
        EffectOp effect0 = new EffectOp
        {
            type = EffectType.DrawCards,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.None,
                participantSelectionMode = ParticipantSelectionMode.None,
                filter = new AllCondition(),
                maxCandidateCount = new NoneValue(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.PointCardsInDeck,
                participantSelectionMode = ParticipantSelectionMode.None,
                filter = new AllCondition(),
                maxCandidateCount = new NoneValue(),
                maxSelectCount = new NoneValue()
            },
            value = new ConstValue
            {
                value = 1
            }
        };
        EffectOp effect1 = new EffectOp
        {
            type = EffectType.DrawCards,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.None,
                participantSelectionMode = ParticipantSelectionMode.None,
                filter = new AllCondition(),
                maxCandidateCount = new NoneValue(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.PointCardsInDeck,
                participantSelectionMode = ParticipantSelectionMode.All,
                filter = new AllCondition(),
                maxCandidateCount = new NoneValue(),
                maxSelectCount = new NoneValue()
            },
            value = new ConstValue
            {
                value = 1
            }
        };
        SkillCard tmp = new SkillCard
        {
            id = 999,
            name = "抽牌",
            description = "抽一张牌，再抽一张牌",
            point = 1,
            type = CardType.Skill,
            count = 1,
            effects = new List<EffectOp> { effect0, effect1 }
        };

        List<SkillCard> cards = new List<SkillCard> { tmp };
        CardJsonUtility.ConvertCardsToJson(cards, "SkillCards.json");
    }

    [ContextMenu("Test PointCard To File")]
    public void Test1()
    {
        List<PointCard> cards = new List<PointCard>();

        for (int i = 1; i <= 10; i++)
        {
            PointCard card = new PointCard
            {
                id = i,
                name = $"点数牌{i}",
                description = $"点数牌{i}",
                point = i,
                type = CardType.Point,
                count = 2,
                effects = new List<EffectOp>()
            };
            cards.Add(card);
        }

        CardJsonUtility.ConvertCardsToJson(cards, "PointCards.json");
    }
}
