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

    public GameObject CreateCardInstace(int cardId, int instaceId, Vector3 position, Quaternion rotation)
    {
        CardType cardType = CardDatabase.Get(cardId).type;

        GameObject cardInstace = null;
        switch (cardType)
        {
            case CardType.Point:
                cardInstace = Instantiate(pointCardInstancePrefab, pointCardSpawnPosition, rotation);
                cardInstace.GetComponent<PointCardInstance>().InitCardInstance(cardId, instaceId);
                break;
            case CardType.Skill:
                cardInstace = Instantiate(skillCardInstancePrefab, position, rotation);
                SkillCardInstance sci = cardInstace.GetComponent<SkillCardInstance>();
                sci.InitCardInstance(cardId, instaceId);
                break;
        }

        cardInstace.transform.localScale = Vector3.zero;
        cardInstace.transform.DOScale(Vector3.one * scaleFactor, 0.15f);
        return cardInstace;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointCardSpawnPosition, 0.05f);
    }
}
