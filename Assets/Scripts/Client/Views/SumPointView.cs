using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SumPointView : MonoBehaviour
{
    public TextMeshPro sumPointTMP;

    public void ChangeSum(int sum, bool isOpponent)
    {
        string text = isOpponent ? sum.ToString() + " + ?" : sum.ToString();
        sumPointTMP.text = text;
    }
}
