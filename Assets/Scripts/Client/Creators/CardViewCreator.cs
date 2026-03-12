using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private GameObject skillCardInstancePrefab;
    [SerializeField] private GameObject pointCardInstancePrefab;

    [SerializeField] private Vector3 pointCardSpawnPosition;

    public float scaleFactor = 0.4f;

    public GameObject CreateCardInstance(int cardId, int instaceId, Vector3 position, Quaternion rotation)
    {
        CardType cardType = CardDatabase.Get(cardId).type;

        GameObject cardObj = null;
        CardInstance cardInstance = null;
        switch (cardType)
        {
            case CardType.Point:
                cardObj = Instantiate(pointCardInstancePrefab, pointCardSpawnPosition, rotation);
                cardInstance = cardObj.GetComponent<PointCardInstance>();
                break;
            case CardType.Skill:
                cardObj = Instantiate(skillCardInstancePrefab, position, rotation);
                cardInstance = cardObj.GetComponent<SkillCardInstance>();
                break;
        }

        cardInstance.InitCardInstance(cardId, instaceId);
        cardObj.transform.localScale = Vector3.zero;
        cardObj.transform.DOScale(Vector3.one * cardInstance.localScaleFactor, 0.15f);
        return cardObj;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointCardSpawnPosition, 0.05f);
    }
}
