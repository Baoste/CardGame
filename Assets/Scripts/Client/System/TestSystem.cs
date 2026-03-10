using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
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
            ClientCommand.DrawSkillCard();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            GameObject instance = CardViewCreator.Instance.CreateCardInstance(1, 99999, transform.position, Quaternion.identity);
            StartCoroutine(ResolveZoneView.AddCard(instance, -1));
        }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    GameObject instace = CardViewCreator.Instance.CreateCardInstace(1, 2, transform.position, Quaternion.identity);
        //    StartCoroutine(boardView.AddCard(instace, 1));
        //}
    }
}
