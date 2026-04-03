using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ViewAnimController : MonoBehaviour
{
    [Header("Point Deck Cover Anim Settings")]
    [SerializeField] private Transform pointCardsDeckCoverPivot;
    [SerializeField] private Transform pointCardsDeck;

    [Header("Mine Skill Deck Cover Anim Settings")]
    [SerializeField] private Transform m_SkillCardsDeckCoverPivot;
    [SerializeField] private Transform m_SkillCardsDeck;
    private Vector3 m_SkillDeckOriginalPosition;

    [Header("Opponent Skill Deck Cover Anim Settings")]
    [SerializeField] private Transform op_SkillCardsDeckCoverPivot;
    [SerializeField] private Transform op_SkillCardsDeck;
    private Vector3 op_SkillDeckOriginalPosition;


    private void Start()
    {
        m_SkillDeckOriginalPosition = m_SkillCardsDeck.position;
        m_SkillCardsDeck.position = m_SkillDeckOriginalPosition - m_SkillCardsDeck.up * 0.3f;
        m_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(-66.11f, 0, 0);

        op_SkillDeckOriginalPosition = op_SkillCardsDeck.position;
        op_SkillCardsDeck.position = op_SkillDeckOriginalPosition - op_SkillCardsDeck.up * 0.3f;
        op_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(ClosePointCardDeckCover());

            m_SkillCardsDeck.position = m_SkillDeckOriginalPosition - m_SkillCardsDeck.up * 0.3f;
            m_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(-66.11f, 0, 0);
            op_SkillCardsDeck.position = op_SkillDeckOriginalPosition - op_SkillCardsDeck.up * 0.3f;
            op_SkillCardsDeckCoverPivot.rotation = Quaternion.Euler(0, 0, 0);
            StartCoroutine(OpenSkillCardDeckCover());
        }
    }

    public IEnumerator ClosePointCardDeckCover()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(pointCardsDeck.DOMoveY(2.02f, 0.5f).SetEase(Ease.OutCubic));
        seq.Append(pointCardsDeckCoverPivot.DORotate(Vector3.zero, 0.25f).SetEase(Ease.InCirc));

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
}
