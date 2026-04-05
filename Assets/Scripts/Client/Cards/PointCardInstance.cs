using Game.Domain;
using TMPro;
using UnityEngine;

public class PointCardInstance : CardInstance
{
    [Header("Component")]
    public TMP_Text pointText;
    public CardState cardState;

    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

    //public bool touchAnotherCard
    //{
    //    get
    //    {
    //        bool value = _touchAnotherCard;
    //        _touchAnotherCard = false;   // 读取后自动重置
    //        return value;
    //    }
    //    set
    //    {
    //        _touchAnotherCard = value;
    //    }
    //}
    //private bool _touchAnotherCard;

    private void Awake()
    {
        matRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public override void InitCardInstance(int cardId, int instanceId)
    {
        base.InitCardInstance(cardId, instanceId);

        localScaleFactor = 0.45f;
        pointText.text = point.ToString();

        Texture2D tex = CardViewCreator.Instance.pointCardTexs[point - 1];
        matRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainMap", tex);
        matRenderer.SetPropertyBlock(mpb);
    }

    public void InitCardState(CardState cardState, bool isOpponent)
    {
        this.cardState = cardState;

        switch (cardState)
        {
            case CardState.None:
                break;
            case CardState.Hole:
                if (isOpponent)
                    pointText.text = "";
                break;
            case CardState.Hidden:
                pointText.text = "";
                // TODO: change mat
                break;
            case CardState.Locked:
                // TODO: change mat
                break;
        }
        matRenderer.SetPropertyBlock(mpb);
    }

    public void ChangeCardState(CardState cardState, bool isOpponent)
    {
        this.cardState = cardState;

        switch (cardState)
        {
            case CardState.None:
            case CardState.Hole:
                break;
            case CardState.Hidden:
                pointText.text = "";
                // TODO: change mat
                break;
            case CardState.Locked:
                // TODO: change mat
                break;
        }
        matRenderer.SetPropertyBlock(mpb);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.GetComponent<PointCardInstance>() != null)
    //    {
    //        touchAnotherCard = true;
    //    }
    //}

    //void OnTriggerStay(Collider other)
    //{
    //    if (other.GetComponent<PointCardInstance>() != null)
    //    {
    //        touchAnotherCard = true;
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponent<PointCardInstance>() != null)
    //    {
    //        touchAnotherCard = false;
    //    }
    //}
}
