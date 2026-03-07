using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private BoardView boardView;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstace(999, 1,transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(instace));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstace(1, 2, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instace, -1));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstace(1, 2, transform.position, Quaternion.identity);
            StartCoroutine(boardView.AddCard(instace, 1));
        }
    }
}
