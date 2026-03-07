using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject instace = CardViewCreator.Instance.CreateCardInstace(999, 1,transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(instace));
        }
    }
}
