using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static bool ManageMouseInputValidity = true;
    public static LayerMask interactMask = LayerMask.GetMask("Card", "HighlightOnly", "Default");

    public static void ChangeInteractMask(params string[] layerNames)
    {
        interactMask = LayerMask.GetMask(layerNames);
    }

    public static void ResetInteractMask()
    {
        interactMask = LayerMask.GetMask("Card", "HighlightOnly", "Default");
    }
}
