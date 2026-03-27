using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointCardInstance : CardInstance
{
    [Header("Component")]
    public TMP_Text pointText;

    private Renderer matRenderer;
    private MaterialPropertyBlock mpb;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PointCardInstance>() != null)
        {
            touchAnotherCard = true;
        }
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
