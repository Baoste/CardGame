using Game.Domain;
using TMPro;
using UnityEngine;

public class PointCardInstance : CardInstance
{
    public CardState cardState { get; private set; }
    private PointCardViewController viewController;

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
        viewController = GetComponent<PointCardViewController>();
    }

    public override void InitCardInstance(int cardId, int instanceId)
    {
        base.InitCardInstance(cardId, instanceId);

        localScaleFactor = 0.45f;
        viewController.SetCardTexture_None(point);
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
                    viewController.SetCardTexture_Hole(point);
                break;
            case CardState.Hidden:
                viewController.SetCardTexture_Hidden();
                // TODO: change mat
                break;
            case CardState.Locked:
                // TODO: change mat
                break;
        }
    }

    public void ChangeCardState(CardState cardState, bool isOpponent)
    {
        this.cardState = cardState;

        switch (cardState)
        {
            case CardState.None:
                viewController.SetCardTexture_None(point);
                break;
            case CardState.Hole:
                break;
            case CardState.Hidden:
                viewController.SetCardTexture_Hidden();
                // TODO: change mat
                break;
            case CardState.Locked:
                // TODO: change mat
                break;
        }
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
