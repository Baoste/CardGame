using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private ResolveZoneView ResolveZoneView;
    private Stack<GameObject> objs = new Stack<GameObject>();
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
            if (card <= 5)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98, transform.position, Quaternion.identity);
                StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, false));
                objs.Push(instance);
            }
            else if (card <= 10)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98, transform.position, Quaternion.identity);
                StartCoroutine(boardView.AddCard(instance, 99, false));
                objs.Push(instance);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objs.Count > 0)
            {
                StartCoroutine(SceneViewManager.boardView.RemoveCard(objs.Pop()));
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(SceneViewManager.boardView.RemoveAllCards());
            objs.Clear();
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
