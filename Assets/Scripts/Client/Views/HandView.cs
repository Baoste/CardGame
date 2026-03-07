using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<GameObject> skillCardInstances = new();

    public IEnumerator AddCard(GameObject instance)
    {
        skillCardInstances.Add(instance);
        yield return UpdateCardPositions(0.15f);
    }
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (skillCardInstances.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cardSpacing * (skillCardInstances.Count - 1) / 2f);
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < skillCardInstances.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            skillCardInstances[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            skillCardInstances[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new  WaitForSeconds(duration);
    }
}
