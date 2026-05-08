
using System.Collections.Generic;
using UnityEngine;

public static class SceneViewManager
{
    public static BoardView boardView;
    public static ResolveZoneView resolveZoneView;
    public static ResolveZoneView peekZoneView;
    public static HandView myHandView;
    public static HandView opponentHandView;
    public static ExecuteCardView myExecuteCardView;
    public static ExecuteCardView opponentExecuteCardView;
    public static RoleView roleView;
    public static RevealView myRevealButtonView;
    public static RevealView opponentRevealButtonView;
    public static TurnLightView myTurnLightView;
    public static TurnLightView opponentTurnLightView;
    public static ChipView myChipView;
    public static ChipView opponentChipView;
    public static SumPointView mySumPointView;
    public static SumPointView opponentSumPointView;
    public static ActionPointView myActionPointView;
    public static ActionPointView opponentActionPointView;
    public static ViewAnimController viewAnimController;
    public static EndTurnView endTurnView;
    public static TurnIndicator turnIndicator;

    private static IEnumerable<IViewClear> views => new IViewClear[]
    {
            boardView,
            resolveZoneView,
            peekZoneView,
            myHandView,
            opponentHandView,
            myRevealButtonView,
            opponentRevealButtonView,
            myTurnLightView,
            opponentTurnLightView,
            turnIndicator
    };

    public static void ClearViews()
    {
        foreach(IViewClear view in views)
        {
            view.ClearView();
        }
    }
}