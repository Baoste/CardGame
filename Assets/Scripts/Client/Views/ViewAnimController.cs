using Cinemachine;
using DG.Tweening;
using FishNet.Example.ColliderRollbacks;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAnimController : MonoBehaviour
{
    [HideInInspector] public CinemachineImpulseSource ImpulseSource;

    [Header("Table Plane Anim Settings")]
    [SerializeField] public TablePlaneMatManager TablePlaneMatManager;
    [SerializeField] public ParticleSystem scannerVFX;
    [SerializeField] public MirzaBeig.LightningVFX.DemoManager LightingManager;

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

    [Header("Final Win Animation")]
    [SerializeField] private Animator finalWinAC;
    [SerializeField] public Transform ChipMoveTransformParent;

    private void Start()
    {
        ImpulseSource = GetComponent<CinemachineImpulseSource>();

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

    public IEnumerator PlayGameEndAnim()
    {
        // 充电
        AudioManager.Instance.Play("SettleUp_ChargeUp");
        TablePlaneMatManager.SetFirstMaterial("Charge");
        TablePlaneMatManager.PlayPlaneAnim(1.5f, 1f, 0.2f);

        // 清空桌面
        // 清空动画。可以打一个波，把手牌和底牌都摧毁
        yield return new WaitForSeconds(2f);
        Transform root = CardViewCreator.Instance.transform;
        foreach (Transform child in root)
        {
            yield return null;
            IDiscardPresentation discardPresentation = child.gameObject.GetComponent<IDiscardPresentation>();
            discardPresentation?.DiscardPlay();
        }

        yield return new WaitForSeconds(0.4f);
        scannerVFX.Play();
        // ImpulseSource.GenerateImpulseWithVelocity(Vector3.up * 0.05f);


        // 准备下一把
        yield return new WaitForSeconds(2f);
        SceneViewManager.ClearViews();
        AudioManager.Instance.Play("SettleUp_OpenCover");
        StartCoroutine(OpenPointCardDeckCover());
        StartCoroutine(CloseSkillCardDeckCover());
        // StartCoroutine(OpenChipCover());
        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().startText.SetActive(true);
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
        SceneViewManager.myHandView.panelAnimator.SetTrigger("Pop");
    }

    public IEnumerator CloseSkillCardDeckCover()
    {
        SceneViewManager.myHandView.panelAnimator.SetTrigger("Retract");
        yield return new WaitForSeconds(0.3f);

        Sequence seq = DOTween.Sequence();
        seq.Append(m_SkillCardsDeck.DOMove(m_SkillDeckOriginalPosition - m_SkillCardsDeck.up * 0.3f, 0.5f).SetEase(Ease.InBack));
        seq.Join(m_SkillCardsDeckCoverPivot.DORotate(new Vector3(-66.11f, 0, 0), 0.5f).SetEase(Ease.InBack));
        seq.Join(op_SkillCardsDeck.DOMove(op_SkillDeckOriginalPosition - op_SkillCardsDeck.up * 0.3f, 0.5f).SetEase(Ease.InBack));
        seq.Join(op_SkillCardsDeckCoverPivot.DORotate(Vector3.zero, 0.5f).SetEase(Ease.InBack));

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

    public IEnumerator PlayMatchEndAnim(bool isWin)
    {
        finalWinAC.updateMode = AnimatorUpdateMode.AnimatePhysics;
        if (ClientGameState.playerSlot == 0)
            finalWinAC.SetTrigger(isWin ? "WomanWin" : "WomanLose");
        else
            finalWinAC.SetTrigger(isWin ? "ManWin" : "ManLose");
        yield return null;
    }
}
