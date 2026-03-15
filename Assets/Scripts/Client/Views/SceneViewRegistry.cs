using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewRegistry : MonoBehaviour
{
    public BoardView boardView;
    public ResolveZoneView resolveZoneView;

    public HandView myHandView;
    public HandView opponentHandView;
    public ExecuteCardView myExecuteCardView;
    public ExecuteCardView opponentExecuteCardView;

    private void Awake()
    {
        SceneViewManager.boardView = boardView;
        SceneViewManager.resolveZoneView = resolveZoneView;
        SceneViewManager.myHandView = myHandView;
        SceneViewManager.opponentHandView = opponentHandView;
        SceneViewManager.myExecuteCardView = myExecuteCardView;
        SceneViewManager.opponentExecuteCardView = opponentExecuteCardView;
    }
}
