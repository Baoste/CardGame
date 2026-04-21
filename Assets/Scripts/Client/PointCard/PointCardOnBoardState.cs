using TMPro;
using UnityEngine;
using System.Collections;

public class PointCardOnBoardState : PointCardState
{
    private float fadeDuration = 0.5f;
    private GameObject pointsRoot;
    private TMP_Text pointsText;
    private Color baseColor;

    private Coroutine fadeCoroutine;

    public PointCardOnBoardState(PointCardStateMachine stateMachine, PointCardController pointCard, string animatorName) : base(stateMachine, pointCard, animatorName)
    {
        pointsRoot = pointCard.viewController.pointText.transform.parent.gameObject;
        pointsText = pointCard.viewController.pointText;
        baseColor = pointsText.color;
    }

    public override void Enter()
    {
        base.Enter();
        SetAlpha(0f);
        pointsRoot.SetActive(false);
        fadeCoroutine = null;
    }

    public override void Exit()
    {
        base.Exit();
        SetAlpha(0f);
        pointsRoot.SetActive(false);
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();

        if (pointsText == null || pointsRoot == null) return;

        pointsText.text = pointCard.viewController.pointText.text;

        if (fadeCoroutine != null)
            pointCard.StopCoroutine(fadeCoroutine);

        pointsRoot.SetActive(true);
        fadeCoroutine = pointCard.StartCoroutine(FadeTo(1f));
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();

        if (pointsText == null || pointsRoot == null) return;

        if (fadeCoroutine != null)
            pointCard.StopCoroutine(fadeCoroutine);

        fadeCoroutine = pointCard.StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = pointsText.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutAndDisable()
    {
        float startAlpha = pointsText.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, 0f, t);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        pointsRoot.SetActive(false);
        fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (pointsText == null) return;

        Color c = baseColor;
        c.a = alpha;
        pointsText.color = c;
    }
}
