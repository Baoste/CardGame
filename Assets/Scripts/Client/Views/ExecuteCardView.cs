using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static MeshDestroy;

public class ExecuteCardView : MonoBehaviour
{
    [Header("Execute Position")]
    [SerializeField] private Vector3 executePosition;
    [SerializeField] private float fallHeight;
    private Vector3 fallPosition;

    [SerializeField] private VisualEffect executeVFX;
    private GameObject executedCard;

    private void Start()
    {
        executeVFX.Stop();
        fallPosition = executePosition + transform.up * fallHeight;
    }

    public IEnumerator MoveToFallPosition(GameObject card, bool isOpponent)
    {
        card.GetComponent<Outline>().Enable = 0f;

        if (isOpponent)
        {
            Vector3 readyPosition = card.transform.position + card.transform.forward * 0.5f;
            readyPosition += card.transform.up * 1f;
            Sequence seq = DOTween.Sequence();
            seq.Append(card.transform.DOMove(readyPosition, 0.8f).SetEase(Ease.InOutCubic));

            yield return seq.WaitForCompletion();
        }
        yield break;
    }

    public IEnumerator MoveToExecutePosition(GameObject card)
    {
        card.GetComponent<Outline>().Enable = 0f;

        StartCoroutine(SceneViewManager.myHandView.RemoveCard(card));
        StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(card));

        Sequence seq = DOTween.Sequence();

        Quaternion targetRot = Quaternion.LookRotation(transform.forward, -transform.right);
        seq.Append(card.transform.DOMove(fallPosition, 0.8f).SetEase(Ease.InOutCubic));
        seq.Join(card.transform.DORotateQuaternion(targetRot, 0.8f).SetEase(Ease.InOutQuad));

        Vector3 start = fallPosition;
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

        seq.Append(card.transform.DOMove(backPos, 0.1f));
        seq.Append(card.transform.DOMove(readyPos, 0.3f).SetEase(Ease.InExpo));
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() =>
        {
            StartCoroutine(SceneViewManager.boardView.ShakeCards());
        });
        seq.Append(card.transform.DOMove(spingPos, 0.08f).SetEase(Ease.OutCubic));
        seq.AppendCallback(() =>
        {
            executeVFX.SetVector4("Color", card.GetComponent<SkillCardInstance>().vfxColor);
            executeVFX.Play();
        });
        seq.Append(card.transform.DOMove(executePosition, 0.04f).SetEase(Ease.InCubic));

        yield return seq.WaitForCompletion();

        ClientEffectContext.isExecutingSkillCard = false;
    }

    public void DestroyCard(GameObject instance)
    {
        if (executedCard != null)
            StartCoroutine(_DestroyCard(instance));
        else
            executedCard = instance;
    }

    private IEnumerator _DestroyCard(GameObject instance)
    {
        MeshDestroy mesh = executedCard.GetComponentInChildren<MeshDestroy>();
        GameObject tmp = mesh.transform.parent.gameObject;
        mesh.transform.parent.parent = transform.parent.parent;

        List<PartMesh> submeshes = mesh.DestroyMesh(4);
        Destroy(tmp);
        Destroy(executedCard);

        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(0.01f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSecondsRealtime(1f);
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            foreach (PartMesh part in submeshes)
                part.GameObject.GetComponent<DissolutionController>().DestroySelf();
        });
        yield return seq.WaitForCompletion();

        executedCard = instance;
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
