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

        EffectOp effect3 = new EffectOp
        {
            type = EffectType.DrawPointToResolve,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.PointCardsInDeck,
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new ConstValue
                {
                    value = 3
                }
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.CardsToResolve,
                participantSelectionMode = new SelectionModeNone(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            value = new NoneValue()
        };
        EffectOp effect4 = new EffectOp
        {
            type = EffectType.Move,
            source = new ParticipantSpec
            {
                participantType = ParticipantType.CardsToResolve,
                participantSelectionMode = new SelectionModeChoose(),
                filter = new AllCondition(),
                maxSelectCount = new ConstValue
                {
                    value = 1
                }
            },
            target = new ParticipantSpec
            {
                participantType = ParticipantType.MyBoardZone,
                participantSelectionMode = new SelectionModeAll(),
                filter = new NoneCondition(),
                maxSelectCount = new NoneValue()
            },
            value = new NoneValue()
        };

        SkillCard tmp0 = new SkillCard
        {
            id = 999,
            name = "抽1牌",
            description = "让对面抽一张牌",
            point = 1,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect0 }
        };

        SkillCard tmp1 = new SkillCard
        {
            id = 9999,
            name = "弃对方1牌",
            description = "选择对面场上的一张牌丢弃",
            point = 1,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect1 }
        };

        SkillCard tmp2 = new SkillCard
        {
            id = 99999,
            name = "点数-1",
            description = "使场上的一张牌的点数-1",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect2 }
        };

        SkillCard tmp3 = new SkillCard
        {
            id = 999999,
            name = "抽3选1",
            description = "抽取三张牌，选择一张牌到自己场上",
            point = 0,
            type = CardType.Skill,
            count = 2,
            effects = new List<EffectOp> { effect3, effect4 }
        };

        List<SkillCard> cards = new List<SkillCard> { tmp0, tmp1, tmp2, tmp3 };
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
