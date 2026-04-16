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
    public MeshRenderer meshRenderer;
    public Material defaultMaterial { get; private set; }

    public GameObject infoPannel;
    public TMP_Text info;

    public Vector3 originalPos;

    public void Awake()
    {
        //nameText = transform.Find("Name").GetComponent<TMP_Text>();
        //descriptionText = transform.Find("Description").GetComponent<TMP_Text>();
        //pointText = transform.Find("Point").GetComponent<TMP_Text>();
    }

    public override void InitCardInstance(int cardId, int instaceId)
    {
        base.InitCardInstance(cardId, instaceId);

        this.cardId = cardId;

        cardName = CardDatabase.Get(cardId).name;
        description = CardDatabase.Get(cardId).description;
        effects = CardDatabase.Get(cardId).effects;

        nameText.text = cardName;
        descriptionText.text = description;
        pointText.text = point.ToString();

        defaultMaterial = meshRenderer.sharedMaterial;

        infoPannel.SetActive(false);
        info.text = description;
    }

    public void ShowInfo()
    {
        infoPannel.SetActive(true);
    }
}
