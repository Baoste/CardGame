using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealButtonView : MonoBehaviour
{
    [SerializeField] private GameObject revealButton;
    private Vector3 hidePosition;
    private Vector3 showPosition;
    
    public bool isEnabled = false;

    public void Start()
    {
        showPosition = revealButton.transform.localPosition;
        hidePosition = showPosition + Vector3.down * 0.1f;

        revealButton.transform.localPosition = hidePosition;
        revealButton.GetComponent<ButtonTest>().SetOriginalPosition(showPosition);
        revealButton.GetComponent<ButtonTest>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
    }

    public void ShowButton()
    {
        isEnabled = true;
        revealButton.transform.DOLocalMove(showPosition, 0.5f);
        revealButton.GetComponent<ButtonTest>().enabled = true;
        revealButton.GetComponent<Collider>().enabled = true;
    }

    public void ShowRandom()
    {
        // TODO: 随机引爆装置
        isEnabled = true;
    }

    public IEnumerator RandomAnimation(bool reveal)
    {
        if (!isEnabled) yield break;

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
