using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCardView : MonoBehaviour
{
    [Header("Execute Position")]
    [SerializeField] private Vector3 executePosition;
    [SerializeField] private float fallHeight;
    private Vector3 fallPosition;

    private void Start()
    {
        fallPosition = executePosition + transform.up * fallHeight;
    }

    public IEnumerator MoveToFallPosition(GameObject card)
    {
        card.GetComponent<SkillCardMouseTilt>().ResetBaseRotation();
        Quaternion targetRot = Quaternion.LookRotation(transform.forward, transform.right);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOMove(fallPosition, 0.8f));
        seq.Join(card.transform.DORotateQuaternion(targetRot, 0.6f));

        yield return seq.WaitForCompletion();
        CommandExecutionState<PlayAnimationCommand>.IsDone = true;
    }

    public IEnumerator MoveToExecutePosition(GameObject card)
    {
        card.GetComponent<SkillCardDraggable>().executed = true;
        StartCoroutine(SceneViewManager.myHandView.RemoveCard(card));
        StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(card));

        Vector3 start = card.transform.position;
        Vector3 dir = (executePosition - start).normalized;     // 朝下
        // 小回撤
        float backDist = 0.1f;
        Vector3 backPos = start - dir * backDist;
        // 卡顿插入前
        float readyDist = 0.05f;
        Vector3 readyPos = executePosition - dir * readyDist;
        // 插入后回弹
        float spingDist = 0.02f;
        Vector3 spingPos = executePosition + dir * spingDist;

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOMove(backPos, 0.1f));
        seq.Append(card.transform.DOMove(readyPos, 0.3f).SetEase(Ease.InExpo));
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() =>
        {
            StartCoroutine(SceneViewManager.boardView.ShakeCards());
        });
        seq.Append(card.transform.DOMove(spingPos, 0.08f).SetEase(Ease.OutCubic));
        seq.Append(card.transform.DOMove(executePosition, 0.04f).SetEase(Ease.InCubic));

        yield return seq.WaitForCompletion();
        CommandExecutionState<PlayAnimationCommand>.IsDone = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(executePosition, 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(executePosition + transform.up * fallHeight, executePosition);
        Gizmos.DrawLine(executePosition, executePosition + transform.right * 0.1f);
    }
}
