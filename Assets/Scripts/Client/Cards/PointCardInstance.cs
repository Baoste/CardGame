using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointCardInstance : CardInstance
{
    [Header("Component")]
    public TMP_Text pointText;

    public bool touchAnotherCard
    {
        get
        {
            bool value = _touchAnotherCard;
            _touchAnotherCard = false;   // 读取后自动重置
            return value;
        }
        set
        {
            _touchAnotherCard = value;
        }
    }
    private bool _touchAnotherCard;

    public void Awake()
    {
    }

    public override void InitCardInstance(int cardId, int instaceId)
    {
        base.InitCardInstance(cardId, instaceId);

        pointText.text = point.ToString();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PointCardInstance>() != null)
        {
            touchAnotherCard = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PointCardInstance>() != null)
        {
            touchAnotherCard = false;
        }
    }
}
