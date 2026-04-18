using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToStartGame : MonoBehaviour, IMouseDown
{
    public GameObject startText;
    public bool isEnabled;

    public void MouseDown()
    {
        if (!isEnabled || ClientGameState.Instance.isStart)
            return;
        StartCoroutine(ClientCommand.StartGame());
        startText.SetActive(false);
    }
}
