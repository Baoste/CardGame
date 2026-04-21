using DG.Tweening;
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
    public GameObject infoPannel;
    public TMP_Text info;

    [Header("Anim")]
    [ColorUsage(true, true)]
    public Color[] vfxColors;

    public Material defaultMaterial { get; private set; }

    public Vector3 originalPos;
    public Color vfxColor {  get; private set; }


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

        vfxColor = vfxColors[point];
    }

    public void ShowInfo()
    {
        infoPannel.SetActive(true);
        infoPannel.transform.DOKill();
        infoPannel.transform.DOLocalMoveX(0.6f, 0.3f).SetEase(Ease.OutBack);
    }

    public void HideInfo()
    {
        infoPannel.transform.DOKill();
        infoPannel.transform.DOLocalMoveX(0f, 0.3f);
        infoPannel.SetActive(false);
    }
}
