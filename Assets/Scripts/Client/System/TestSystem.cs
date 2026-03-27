using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private ResolveZoneView ResolveZoneView;
    private GameObject obj;
    private int turn = 0;
    private int card = 1;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //ClientCommand.DrawSkillCard();
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(6219298, 9999, transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(instance));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99999, transform.position, Quaternion.identity);
            StartCoroutine(ResolveZoneView.AddCard(instance, -1, true));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject instance = null;
            if (card <= 5)
            {
                instance = CardViewCreator.Instance.CreateCardInstance(card++, 98, transform.position, Quaternion.identity);
                StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, false));
            }
            else if (card <= 10)
            {
                instance = CardViewCreator.Instance.CreateCardInstance(card++, 98, transform.position, Quaternion.identity);
                StartCoroutine(boardView.AddCard(instance, 99, false));
            }
            obj = instance;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SceneViewManager.boardView.RemoveCard(obj));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, true));
            instance = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, 99, true));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneViewManager.myRevealButtonView.ShowButton(true);
            SceneViewManager.opponentRevealButtonView.ShowRandom();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneViewManager.myTurnLightView.SetLight(++turn);
        }
    }
}
