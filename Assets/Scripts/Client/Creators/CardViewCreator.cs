using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private GameObject cardViewPrefab;

    public GameObject CreateCardInstace(int cardId, int instaceId, Vector3 position, Quaternion rotation)
    {
        CardType cardType = CardDatabase.Get(cardId).type;

        GameObject cardInstace = null;
        switch (cardType)
        {
            case CardType.Point:
                cardInstace = Instantiate(cardViewPrefab, position, rotation);
                cardInstace.GetComponent<PointCardInstance>().InitCardInstance(cardId, instaceId);
                break;
            case CardType.Skill:
                cardInstace = Instantiate(cardViewPrefab, position, rotation);
                SkillCardInstance sci = cardInstace.GetComponent<SkillCardInstance>();
                sci.InitCardInstance(cardId, instaceId);
                break;
        }

        cardInstace.transform.localScale = Vector3.zero;
        cardInstace.transform.DOScale(Vector3.one, 0.15f);
        return cardInstace;
    }
}
