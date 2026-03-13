using DG.Tweening;
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
    }

    public IEnumerator MoveToExecutePosition(GameObject card)
    {
        Vector3 start = card.transform.position;
        Vector3 dir = (executePosition - start).normalized;
        float backDist = 0.1f;
        Vector3 backPos = start - dir * backDist;

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOMove(backPos, 0.1f));
        seq.Append(card.transform.DOMove(executePosition, 0.3f).SetEase(Ease.InExpo));

        yield return seq.WaitForCompletion();
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
