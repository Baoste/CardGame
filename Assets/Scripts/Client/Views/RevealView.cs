using DG.Tweening;
using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealView : MonoBehaviour, IViewClear
{
    [SerializeField] private GameObject revealButton;
    [SerializeField] private GameObject slotMachine;

    private Vector3 BtnHidePosition;
    private Vector3 BtnShowPosition;
    
    private bool isRandomEnabled = false;

    public void Start()
    {
        slotMachine.transform.rotation = Quaternion.Euler(-110, 0, 0);

        BtnShowPosition = revealButton.transform.localPosition;

        BtnHidePosition = BtnShowPosition + Vector3.down * 0.1f;
        revealButton.transform.localPosition = BtnHidePosition;

        revealButton.GetComponent<RevealButton>().SetOriginalPosition(BtnShowPosition);
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
    }

    public void ClearView()
    {
        revealButton.transform.DOKill();
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
        isRandomEnabled = false;
        revealButton.transform.DOLocalMove(BtnHidePosition, 0.5f);
    }

    public void ShowButton(bool canClick)
    {
        revealButton.transform.DOLocalMove(BtnShowPosition, 0.5f);
        if (canClick)
        {
            revealButton.GetComponent<RevealButton>().enabled = true;
            revealButton.GetComponent<Collider>().enabled = true;
        }
    }

    public void HideButton()
    {
        revealButton.transform.DOKill();
        revealButton.GetComponent<RevealButton>().enabled = false;
        revealButton.GetComponent<Collider>().enabled = false;
        revealButton.transform.DOLocalMove(BtnHidePosition, 0.5f);
    }

    public void ShowRandom()
    {
        isRandomEnabled = true;
    }

    public IEnumerator RandomAnimation(bool reveal)
    {
        if (!isRandomEnabled) yield break;

        // µôÂäËæ»úÒý±¬×°ÖÃ
        slotMachine.SetActive(true);
        yield return slotMachine.GetComponent<SlotMachine>().PlayAnimation(reveal);
        slotMachine.SetActive(false);

        if (reveal && ClientGameState.playerSlot != -1 && ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
        {
            ClientCommand.RevealCardsAndScore();
        }
    }

    private void OnDrawGizmos()
    {
    }
}
