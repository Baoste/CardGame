using UnityEngine;
using Cinemachine;

[CreateAssetMenu(menuName = "Cinemachine/Envelope Shake Signal")]
public class EnvelopeShakeSignal : SignalSourceAsset
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float frequency = 18f;

    [SerializeField] private Vector3 positionAmplitude = new Vector3(0.1f, 0.15f, 0f);
    [SerializeField] private Vector3 rotationAmplitude = new Vector3(2f, 1f, 0.5f);

    public override float SignalDuration => duration;

    public override void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
    {
        float x = (Mathf.PerlinNoise(timeSinceSignalStart * frequency, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(timeSinceSignalStart * frequency, 10f) - 0.5f) * 2f;
        float z = (Mathf.PerlinNoise(timeSinceSignalStart * frequency, 20f) - 0.5f) * 2f;

        pos = new Vector3(
            x * positionAmplitude.x,
            y * positionAmplitude.y,
            z * positionAmplitude.z
        );

        Vector3 euler = new Vector3(
            x * rotationAmplitude.x,
            y * rotationAmplitude.y,
            z * rotationAmplitude.z
        );

        rot = Quaternion.Euler(euler);
    }
}