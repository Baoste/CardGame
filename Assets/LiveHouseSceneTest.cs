using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveHouseSceneTest : MonoBehaviour
{
    [SerializeField] private StairMovingCamera stairMovingCamera;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stairMovingCamera.PlayIntro();
            Debug.Log("PlayIntro called");
        }
    }
}
