using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawCardTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ProcessDispatcher.Register("DrawCardTest", DrawCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard(object[] parameters)
    {
        Card card = CardDatabase.Get((int)parameters[0]);
        if (card == null)
        {
            Debug.Log($"Card with id {(int)parameters[0]} not found");
            return;
        }

        Vector3 position = new Vector3(0, 0, 0);
        Quaternion rotation = Quaternion.identity;

        GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        GameObject newCard = Instantiate(cardPrefab, position, rotation);
        newCard.GetComponentInChildren<TextMeshPro>().text = card.name;
    }
}
