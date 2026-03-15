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
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(999, 9999, transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(instance));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            GameObject instance = CardViewCreator.Instance.CreateCardInstance(1, 99999, transform.position, Quaternion.identity);
            StartCoroutine(ResolveZoneView.AddCard(instance, -1));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstance(1, 98, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instace, ClientGameState.playerSlot));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstance(2, 99, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instace, 99));
        }

    }
}
