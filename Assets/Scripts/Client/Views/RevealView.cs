using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealView : MonoBehaviour, IViewClear
{
    [SerializeField] private GameObject revealButton;
    [SerializeField] private GameObject revealRandom;

    // private Vector3 BtnHidePosition;
    private Vector3 BtnShowPosition;
    private Vector3 RdmHidePosition;
    private Vector3 RdmShowPosition;
    
    private bool isRandomEnabled = false;

    public void Start()
    {
        RdmShowPosition = revealRandom.transform.localPosition;
        RdmHidePosition = RdmShowPosition + Vector3.down * 0.1f;
        revealRandom.transform.localPosition = RdmHidePosition;

        BtnShowPosition = revealButton.transform.localPosition;
        // BtnHidePosition = BtnShowPosition + Vector3.down * 0.1f;
        // revealButton.transform.localPosition = BtnHidePosition;
        revealButton.GetComponent<RevealButton>().SetOriginalPosition(BtnShowPosition);
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
    }

    public void ClearView()
    {
        revealRandom.transform.localPosition = RdmHidePosition;
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
    }

    public void ShowButton(bool canClick)
    {
        // revealButton.transform.DOLocalMove(BtnShowPosition, 0.5f);
        if (canClick)
        {
            revealButton.GetComponent<RevealButton>().enabled = true;
            revealButton.GetComponent<Collider>().enabled = true;
        }
    }

    public void ShowRandom()
    {
        isRandomEnabled = true;
        // TODO: 随机引爆装置
        revealRandom.transform.DOLocalMove(RdmShowPosition, 0.5f);
    }

    public IEnumerator RandomAnimation(bool reveal)
    {
        if (!isRandomEnabled) yield break;

        // TODO: 播放随机选择的dongh
        Sequence seq = DOTween.Sequence();
        // seq.Append();
        yield return seq.WaitForCompletion();

        if (reveal)
        {
            ClientCommand.RevealCardsAndScore();
        }
    }

    private void OnDrawGizmos()
    {
    }
}
