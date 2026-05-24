using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAnimController : MonoBehaviour
{
    [Header("Point Deck Cover Anim Settings")]
    [SerializeField] private Transform pointCardsDeckCoverPivot;
    [SerializeField] private Transform pointCardsDeck;

    [Header("Skill card display")]
    public DisplaySkillCard displaySkillCard;

    [Header("Mine Skill Deck Cover Anim Settings")]
    [SerializeField] private Transform m_SkillCardsDeckCoverPivot;
    [SerializeField] private Transform m_SkillCardsDeck;
    private Vector3 m_SkillDeckOriginalPosition;

    [Header("Opponent Skill Deck Cover Anim Settings")]
    [SerializeField] private Transform op_SkillCardsDeckCoverPivot;
    [SerializeField] private Transform op_SkillCardsDeck;
    private Vector3 op_SkillDeckOriginalPosition;

    [Header("Mine Chip Cover Anim Settings")]
    [SerializeField] private Transform m_ChipCoverPivot;
    [SerializeField] private Transform op_ChipCoverPivot;

    private void Start()
    {
        m_SkillDeckOriginalPosition = m_SkillCardsDeck.position;
        m_SkillCardsDeck.position = m_SkillDeckOriginalPosition - m_SkillCardsDeck.up * 0.3f;
        m_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(-66.11f, 0, 0);

        op_SkillDeckOriginalPosition = op_SkillCardsDeck.position;
        op_SkillCardsDeck.position = op_SkillDeckOriginalPosition - op_SkillCardsDeck.up * 0.3f;
        op_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(0, 0, 0);
    }

    public IEnumerator PlayStartGameAnim(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ClosePointCardDeckCover());
        StartCoroutine(OpenSkillCardDeckCover());
    }

    public IEnumerator PlayGameEndAnim(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(OpenPointCardDeckCover());
        StartCoroutine(CloseSkillCardDeckCover());
        // StartCoroutine(OpenChipCover());
        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().startText.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);
        ClientCommand.StartGame();
    }

    public IEnumerator ClosePointCardDeckCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(pointCardsDeck.DOMoveY(2.02f, 0.5f).SetEase(Ease.OutCubic));
        seq.Append(pointCardsDeckCoverPivot.DORotate(Vector3.zero, 0.25f).SetEase(Ease.InCubic));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator OpenPointCardDeckCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(pointCardsDeckCoverPivot.DORotate(new Vector3(108.62f, 0, 0), 0.25f).SetEase(Ease.InCubic));
        seq.Append(pointCardsDeck.DOMoveY(2.195f, 0.5f).SetEase(Ease.OutCubic));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator OpenSkillCardDeckCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_SkillCardsDeckCoverPivot.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutBack));
        seq.Join(m_SkillCardsDeck.DOMove(m_SkillDeckOriginalPosition, 0.5f).SetEase(Ease.OutBack));
        seq.Join(op_SkillCardsDeckCoverPivot.DORotate(new Vector3(-66.11f, 0, 0), 0.25f).SetEase(Ease.OutBack));
        seq.Join(op_SkillCardsDeck.DOMove(op_SkillDeckOriginalPosition, 0.5f).SetEase(Ease.OutBack));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator CloseSkillCardDeckCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_SkillCardsDeckCoverPivot.DORotate(new Vector3(-66.11f, 0, 0), 0.25f).SetEase(Ease.InBack));
        seq.Join(m_SkillCardsDeck.DOMove(m_SkillDeckOriginalPosition - m_SkillCardsDeck.up * 0.3f, 0.5f).SetEase(Ease.InBack));
        seq.Join(op_SkillCardsDeckCoverPivot.DORotate(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        seq.Join(op_SkillCardsDeck.DOMove(op_SkillDeckOriginalPosition - op_SkillCardsDeck.up * 0.3f, 0.5f).SetEase(Ease.InBack));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator CloseChipCover()
    {
        yield return new WaitForSeconds(1f);

        Sequence seq = DOTween.Sequence();
        seq.Append(m_ChipCoverPivot.DORotate(new Vector3(-49.6f, 0, 0), 0.35f).SetEase(Ease.InCubic));
        seq.Join(op_ChipCoverPivot.DORotate(Vector3.zero, 0.35f).SetEase(Ease.InCubic));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator OpenChipCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(m_ChipCoverPivot.DORotate(Vector3.zero, 0.35f).SetEase(Ease.InCubic));
        seq.Join(op_ChipCoverPivot.DORotate(new Vector3(-49.6f, 0, 0), 0.35f).SetEase(Ease.InCubic));

        yield return seq.WaitForCompletion();
    }
}
