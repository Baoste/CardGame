using System.Collections;
using UnityEngine;
using TMPro;

public class PointCardHover : MonoBehaviour
{
    [Header("References")]
    public GameObject pointsRoot;          // 整个投影对象
    public TextMeshPro pointsText;         // TMP文字
    public PointCardInstance pointCardInstance;

    [Header("Fade")]
    public float fadeDuration = 0.25f;

    private Coroutine fadeCoroutine;
    private Color baseColor;

    private int cardLayerMask;
    private PointCardHover currentHover;

    private void Awake()
    {
        cardLayerMask = LayerMask.GetMask("Card");

        if (pointsText != null)
        {
            baseColor = pointsText.color;
        }

        SetAlpha(0f);
        if (pointsRoot != null)
            pointsRoot.SetActive(false);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, cardLayerMask))
        {
            PointCardHover card = hit.collider.GetComponent<PointCardHover>();

            if (card != currentHover)
            {
                currentHover = card;

                if (card != null)
                    ShowPoints();
                else
                    HidePoints();
            }
        }
        else
        {
            if (currentHover != null)
            {
                currentHover = null;
                HidePoints();
            }
        }
    }

    //private void OnMouseEnter()
    //{
    //    ShowPoints();
    //}

    //private void OnMouseExit()
    //{
    //    HidePoints();
    //}

    public void ShowPoints()
    {
        if (pointsText == null || pointsRoot == null) return;

        pointsText.text = GetComponent<PointCardInstance>().pointText.text;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        pointsRoot.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeTo(1f));
    }

    public void HidePoints()
    {
        if (pointsText == null || pointsRoot == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAndDisable());
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