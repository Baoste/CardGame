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
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.OpponentPointCardsOnBoard,
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            value = new ConstValue
            {
                value = 1
            }
        };

        EffectOp effect1 = new EffectOp
        {
            type = EffectType.Discard,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.None,
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.OpponentPointCardsOnBoard,
                participantSelectionMode = new SelectionModeChoose(),
                filter = new AllCondition(),
                maxSelectCount = new ConstValue
                {
                    value = 1
                }
            },
            value = new NoneValue()
        };

        EffectOp effect2 = new EffectOp
        {
            type = EffectType.ModifyPoint,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.None,
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.OpponentPointCardsOnBoard | ParticipantType.MyPointCardsOnBoard,
                participantSelectionMode = new SelectionModeChoose(),
                filter = new AllCondition(),
                maxSelectCount = new ConstValue
                {
                    value = 1
                }
            },
            value = new ConstValue
            {
                value = -1
            }
        };

        SkillCard tmp0 = new SkillCard
        {
            id = 999,
            name = "抽牌",
            description = "让对面抽一张牌",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect0 }
        };

        SkillCard tmp1 = new SkillCard
        {
            id = 9999,
            name = "弃牌",
            description = "选择对面场上的一张牌丢弃",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect1 }
        };

        SkillCard tmp2 = new SkillCard
        {
            id = 99999,
            name = "改牌",
            description = "使场上的一张牌的点数-1",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect2 }
        };

        List<SkillCard> cards = new List<SkillCard> { tmp0, tmp1, tmp2 };
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
