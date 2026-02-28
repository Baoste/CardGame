using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardTest : MonoBehaviour
{
    public GameObject cardPrefab;
    
    public void OnClick()
    {
        //Vector3 position = new Vector3(0, 0, 0);
        //Quaternion rotation = Quaternion.identity;
        //Debug.Log("DrawCardTest OnClick");

        //InitiateCard(position, rotation);
    }

    public void InitiateCard(Vector3 position, Quaternion rotation, int id, string name, string description, int point)
    {
        GameObject newCard = Instantiate(cardPrefab, position, rotation);
        
    }
}
