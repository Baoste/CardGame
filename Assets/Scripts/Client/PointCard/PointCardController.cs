using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static MeshDestroy;

public class PointCardController : MonoBehaviour, IDiscardPresentation
{
    #region State
    [HideInInspector] public PointCardStateMachine stateMachine;
    [HideInInspector] public PointCardInDeckState inDeckState;
    [HideInInspector] public PointCardOnBoardState onBoardState;
    [HideInInspector] public PointCardDiscardState discardState;
    #endregion

    [Header("VFX")]
    public VisualEffect smokeVFX;

    [HideInInspector] public PointCardInstance instance;
    [HideInInspector] public PointCardViewController viewController;

    public bool isOpponent { get; private set; }

    public void DiscardPlay()
    {
        StartCoroutine(SceneViewManager.boardView.RemoveCard(gameObject));
    }

    private void Awake()
    {
        instance = GetComponent<PointCardInstance>();
        viewController = GetComponent<PointCardViewController>();

        stateMachine = new PointCardStateMachine();
        inDeckState = new PointCardInDeckState(stateMachine, this, "isInDeck");
        onBoardState = new PointCardOnBoardState(stateMachine, this, "isOnBoard");
        discardState = new PointCardDiscardState(stateMachine, this, "isDiscard");
    }

    private void Start()
    {
        // animator = GetComponentInChildren<Animator>();
        GetComponent<PointCardMouseEventHandler>().Init();
        stateMachine.Initialize(inDeckState);

        smokeVFX.Stop();
    }

    public void SetIsOpponent(bool isOpponent)
    {
        this.isOpponent = isOpponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Lazer") && stateMachine.currentState == onBoardState)
        {
            StartCoroutine(DestroyByLazer());
        }
    }

    private IEnumerator DestroyByLazer()
    {
        stateMachine.ChangeState(discardState);
        // mesh destroy
        MeshDestroy mesh = GetComponentInChildren<MeshDestroy>();
        GameObject tmp = mesh.transform.parent.gameObject;
        mesh.transform.parent.parent = transform.parent.parent;

        PointCardInstance point = GetComponentInChildren<PointCardInstance>();
        int cutCascades = 1;
        List<PartMesh> submeshes = mesh.DestroyMesh(cutCascades, 90);
        Destroy(tmp);

        // mesh split
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

        Destroy(gameObject);
    }
}
