using DG.Tweening;
using Game.Domain;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private GameObject skillCardInstancePrefab;
    [SerializeField] private GameObject pointCardInstancePrefab;
    [SerializeField] private GameObject resolveCardInstancePrefab;

    [Header("Point Card Tex")]
    public Texture2D[] pointCardTexs;
    public MaterialMap pointCardStateMatMap;

    public GameObject CreateCardInstance(int cardId, int instanceId)
    {
        CardType cardType = CardDatabase.Get(cardId).type;

        GameObject cardObj = null;
        CardInstance cardInstance = null;
        switch (cardType)
        {
            case CardType.Point:
                cardObj = Instantiate(pointCardInstancePrefab, transform);
                cardInstance = cardObj.GetComponent<PointCardInstance>();
                break;
            case CardType.Skill:
                cardObj = Instantiate(skillCardInstancePrefab, transform);
                cardInstance = cardObj.GetComponent<SkillCardInstance>();
                break;
        }

        cardInstance.InitCardInstance(cardId, instanceId);
        cardObj.transform.localScale = Vector3.one * cardInstance.localScaleFactor;
        return cardObj;
    }

    public GameObject CreateCardResolved(int cardId, int instanceId)
    {
        GameObject cardObj = null;
        CardInstance cardResolve = null;
        
        cardObj = Instantiate(resolveCardInstancePrefab, transform);
        cardResolve = cardObj.GetComponent<PointCardResolve>();

        cardResolve.InitCardInstance(cardId, instanceId);
        cardObj.transform.localScale = Vector3.one * cardResolve.localScaleFactor;
        return cardObj;
    }
}
