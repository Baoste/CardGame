using DG.Tweening;
using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealView : MonoBehaviour, IViewClear
{
    [SerializeField] private GameObject revealButton;
    [SerializeField] private GameObject revealRandom;
    [SerializeField] private GameObject randomRoulette;

    private Vector3 BtnHidePosition;
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

        BtnHidePosition = BtnShowPosition + Vector3.down * 0.1f;
        revealButton.transform.localPosition = BtnHidePosition;

        revealButton.GetComponent<RevealButton>().SetOriginalPosition(BtnShowPosition);
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
    }

    public void ClearView()
    {
        revealRandom.transform.localPosition = RdmHidePosition;
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
        isRandomEnabled = false;
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

        Vector3 rot = new Vector3(0, 360 * 3, 0);
        if (!reveal) rot += new Vector3(0, 90, 0);

        // TODO: 播放随机选择的dongh
        randomRoulette.SetActive(true);
        Quaternion quaternion = randomRoulette.transform.rotation;

        Sequence seq = DOTween.Sequence();
        seq.Append(randomRoulette.transform.DORotate(rot, 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack));

        yield return seq.WaitForCompletion();
        yield return new WaitForSecondsRealtime(1.5f);

        randomRoulette.transform.rotation = quaternion;
        randomRoulette.SetActive(false);

        if (reveal && ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
        {
            ClientCommand.RevealCardsAndScore();
        }
    }

    private void OnDrawGizmos()
    {
    }
}
