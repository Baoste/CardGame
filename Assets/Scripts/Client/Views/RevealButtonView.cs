using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealButtonView : MonoBehaviour
{
    [SerializeField] private GameObject revealButton;
    [SerializeField] private Vector3 hidePosition;
    [SerializeField] public Vector3 showPosition;
    
    public bool isEnabled = false;

    public void Start()
    {
        revealButton.transform.position = hidePosition;
        revealButton.GetComponent<ButtonTest>().SetOriginalPosition(showPosition);
        revealButton.GetComponent<ButtonTest>().enabled = false;
    }

    public void ShowButton()
    {
        isEnabled = true;
        revealButton.transform.DOMove(showPosition, 0.5f);
        revealButton.GetComponent<ButtonTest>().enabled = true;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hidePosition, 0.02f);
        Gizmos.DrawSphere(showPosition, 0.02f);
    }
}
