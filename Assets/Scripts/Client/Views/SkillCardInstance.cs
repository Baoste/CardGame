using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillCardInstance : CardInstance
{
    [Header("Value")]
    public int cardId;
    public string cardName;
    public string description;
    public List<EffectOp> effects;

    [Header("Component")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text pointText;

    public void Awake()
    {
        //nameText = transform.Find("Name").GetComponent<TMP_Text>();
        //descriptionText = transform.Find("Description").GetComponent<TMP_Text>();
        //pointText = transform.Find("Point").GetComponent<TMP_Text>();
    }

    public void InitCardInstance(int cardId, int instaceId)
    {
        this.instanceId = instaceId;
        this.cardId = cardId;

        cardName = CardDatabase.Get(cardId).name;
        description = CardDatabase.Get(cardId).description;
        point = CardDatabase.Get(cardId).point;
        effects = CardDatabase.Get(cardId).effects;

        nameText.text = cardName;
        descriptionText.text = description;
        pointText.text = point.ToString();
    }
}
