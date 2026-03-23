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
            StartCoroutine(ResolveZoneView.AddCard(instance, -1));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 98, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, false));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instance, 99, false));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, true));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, 99, true));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneViewManager.myRevealButtonView.ShowButton();
        }
    }
}
