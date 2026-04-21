using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StepPose
{
    public float time;
    public Vector3 localOffset;
    public Vector3 localEuler;
}

public class StairMovingCamera : MonoBehaviour
{
    [Header("整体移动")]
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float totalDuration = 3f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("局部步态")]
    [SerializeField] private Transform cameraChild;
    [SerializeField] private StepPose[] stepPoses;

    private Vector3 _camBaseLocalPos;
    private Quaternion _camBaseLocalRot;
    private bool _isPlaying;

    private void Awake()
    {
        if (cameraChild != null)
        {
            _camBaseLocalPos = cameraChild.localPosition;
            _camBaseLocalRot = cameraChild.localRotation;
        }

        stepPoses = new StepPose[]
        {
            new StepPose { time = 0.0f, localOffset = new Vector3(0f, 0f, 0f), localEuler = new Vector3(0f, 0f, 0f) },

            new StepPose { time = 1f, localOffset = new Vector3(-0.02f, -0.05f, 0f), localEuler = new Vector3(1.5f, 0f, -1.2f) },
            new StepPose { time = 1.5f, localOffset = new Vector3(0f, 0.01f, 0f), localEuler = new Vector3(0.2f, 0f, 0f) },

            new StepPose { time = 2f, localOffset = new Vector3(0.025f, -0.06f, 0f), localEuler = new Vector3(1.8f, 0f, 1.4f) },
            new StepPose { time = 2.5f, localOffset = new Vector3(0f, 0.01f, 0f), localEuler = new Vector3(0.2f, 0f, 0f) },

            new StepPose { time = 3f, localOffset = new Vector3(-0.018f, -0.045f, 0f), localEuler = new Vector3(1.2f, 0f, -1.0f) },
            new StepPose { time = 3.5f, localOffset = new Vector3(0f, 0f, 0f), localEuler = new Vector3(0f, 0f, 0f) },
        };
    }

    private void Start()
    {
        if (pathPoints != null && pathPoints.Length > 0)
        {
            transform.position = pathPoints[0].position;
            transform.rotation = pathPoints[0].rotation;
        }
    }

    public void PlayIntro()
    {
        if (_isPlaying) return;
        if (pathPoints == null || pathPoints.Length < 2) return;

        StartCoroutine(CoPlayIntro());
    }

    private IEnumerator CoPlayIntro()
    {
        _isPlaying = true;

        float timer = 0f;

        while (timer < totalDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / totalDuration);

            UpdateRootMove(t);
            UpdateStepPose(timer);

            yield return null;
        }

        UpdateRootMove(1f);
        UpdateStepPose(totalDuration);

        _isPlaying = false;
    }

    private void UpdateRootMove(float normalizedTime)
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        float curvedT = moveCurve.Evaluate(normalizedTime);

        float scaled = curvedT * (pathPoints.Length - 1);
        int index = Mathf.FloorToInt(scaled);

        if (index >= pathPoints.Length - 1)
        {
            transform.position = pathPoints[pathPoints.Length - 1].position;
            transform.rotation = pathPoints[pathPoints.Length - 1].rotation;
            return;
        }

        float segmentT = scaled - index;

        Transform from = pathPoints[index];
        Transform to = pathPoints[index + 1];

        transform.position = Vector3.Lerp(from.position, to.position, segmentT);
        transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, segmentT);
    }

    private void UpdateStepPose(float currentTime)
    {
        if (cameraChild == null || stepPoses == null || stepPoses.Length == 0)
            return;

        if (stepPoses.Length == 1)
        {
            ApplyPose(stepPoses[0]);
            return;
        }

        if (currentTime <= stepPoses[0].time)
        {
            ApplyPose(stepPoses[0]);
            return;
        }

        if (currentTime >= stepPoses[stepPoses.Length - 1].time)
        {
            ApplyPose(stepPoses[stepPoses.Length - 1]);
            return;
        }

        for (int i = 0; i < stepPoses.Length - 1; i++)
        {
            StepPose a = stepPoses[i];
            StepPose b = stepPoses[i + 1];

            if (currentTime >= a.time && currentTime <= b.time)
            {
                float t = Mathf.InverseLerp(a.time, b.time, currentTime);

                Vector3 pos = Vector3.Lerp(a.localOffset, b.localOffset, t);
                Vector3 euler = Vector3.Lerp(a.localEuler, b.localEuler, t);

                cameraChild.localPosition = _camBaseLocalPos + pos;
                cameraChild.localRotation = _camBaseLocalRot * Quaternion.Euler(euler);
                return;
            }
        }
    }

    private void ApplyPose(StepPose pose)
    {
        cameraChild.localPosition = _camBaseLocalPos + pose.localOffset;
        cameraChild.localRotation = _camBaseLocalRot * Quaternion.Euler(pose.localEuler);
    }
}
