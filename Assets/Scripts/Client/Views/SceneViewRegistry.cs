using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewRegistry : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private ResolveZoneView resolveZoneView;

    [SerializeField] private HandView myHandView;
    [SerializeField] private HandView opponentHandView;
    [SerializeField] private ExecuteCardView myExecuteCardView;
    [SerializeField] private ExecuteCardView opponentExecuteCardView;

    [SerializeField] private RoleView roleView;
    [SerializeField] private RevealView myRevealButtonView;
    [SerializeField] private RevealView opponentRevealButtonView;

    [SerializeField] private TurnLightView myTurnLightView;
    [SerializeField] private TurnLightView opponentTurnLightView;

    private void Awake()
    {
        SceneViewManager.boardView = boardView;
        SceneViewManager.resolveZoneView = resolveZoneView;
        SceneViewManager.myHandView = myHandView;
        SceneViewManager.opponentHandView = opponentHandView;
        SceneViewManager.myExecuteCardView = myExecuteCardView;
        SceneViewManager.opponentExecuteCardView = opponentExecuteCardView;
        SceneViewManager.roleView = roleView;
        SceneViewManager.myRevealButtonView = myRevealButtonView;
        SceneViewManager.opponentRevealButtonView = opponentRevealButtonView;
        SceneViewManager.myTurnLightView = myTurnLightView;
        SceneViewManager.opponentTurnLightView = opponentTurnLightView;
    }
}
