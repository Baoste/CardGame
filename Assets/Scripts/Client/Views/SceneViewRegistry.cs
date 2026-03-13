using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewRegistry : MonoBehaviour
{
    public BoardView myBoardView;
    public HandView myHandView;
    public ExecuteCardView myExecuteCardView;

    private void Awake()
    {
        SceneViewManager.myBoardView = myBoardView;
        SceneViewManager.myHandView = myHandView;
        SceneViewManager.myExecuteCardView = myExecuteCardView;
    }
}
