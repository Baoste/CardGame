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
            type = EffectType.DrawPoint,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.None,
                participantSelectionMode = ParticipantSelectionMode.None,
                filter = new NoneCondition(),
                maxCandidateCount = new NoneValue(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.OpponentPointCardsOnBoard,
                participantSelectionMode = ParticipantSelectionMode.None,
                filter = new NoneCondition(),
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
            description = "让对面抽一张牌",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect0 }
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
