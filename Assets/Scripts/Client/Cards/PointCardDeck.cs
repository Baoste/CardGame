using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardDeck : MonoBehaviour
{
    [SerializeField] private Vector3 instantiatePosition;

    public IEnumerator EjectDisk(Transform disk, float rotation)
    {
        Vector3 targetPos = Quaternion.AngleAxis(rotation, Vector3.up) * instantiatePosition;
        Sequence seq = DOTween.Sequence();
        seq.Append(disk.DOMove(targetPos, 0.5f));
        yield return seq.WaitForCompletion();
    }

    public IEnumerator EjectDisk(Transform disk, Vector3 position)
    {
        Vector2 a = new Vector2(instantiatePosition.x, instantiatePosition.z);
        Vector2 b = new Vector2(position.x, position.z);
        float angle = Vector2.SignedAngle(b, a);
        Vector3 targetPos = Quaternion.AngleAxis(angle, Vector3.up) * instantiatePosition;
        Sequence seq = DOTween.Sequence();
        seq.Append(disk.DOMove(targetPos, 0.5f));
        yield return seq.WaitForCompletion();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(instantiatePosition, 0.01f);
    }
}
