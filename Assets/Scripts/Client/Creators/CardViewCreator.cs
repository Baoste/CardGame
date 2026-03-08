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

    public float scaleFactor = 0.5f;

    public GameObject CreateCardInstance(int cardId, int instaceId, Vector3 position, Quaternion rotation)
    {
        CardType cardType = CardDatabase.Get(cardId).type;

        GameObject cardInstance = null;
        switch (cardType)
        {
            case CardType.Point:
                cardInstance = Instantiate(pointCardInstancePrefab, pointCardSpawnPosition, rotation);
                cardInstance.GetComponent<PointCardInstance>().InitCardInstance(cardId, instaceId);
                break;
            case CardType.Skill:
                cardInstance = Instantiate(skillCardInstancePrefab, position, rotation);
                SkillCardInstance sci = cardInstance.GetComponent<SkillCardInstance>();
                sci.InitCardInstance(cardId, instaceId);
                break;
        }

        cardInstance.transform.localScale = Vector3.zero;
        cardInstance.transform.DOScale(Vector3.one * scaleFactor, 0.15f);
        return cardInstance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointCardSpawnPosition, 0.05f);
    }
}
