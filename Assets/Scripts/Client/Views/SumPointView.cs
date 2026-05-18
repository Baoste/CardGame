using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SumPointView : MonoBehaviour
{
    public TextMeshPro sumPointTMP;

    public void ChangeSum(int sum, bool isShown)
    {
        string text = isShown ? sum.ToString() : sum.ToString() + " + X";
        sumPointTMP.text = text;
    }
}
