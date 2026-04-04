using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToStartGame : MonoBehaviour, IMouseDown
{
    public void MouseDown()
    {
        if (ClientGameState.Instance.isStart)
            return;
        ClientCommand.StartGame();
    }
}
