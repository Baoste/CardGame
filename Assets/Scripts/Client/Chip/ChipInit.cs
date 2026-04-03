using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipInit : MonoBehaviour
{
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Vector3 instantiatePosition;
    [SerializeField] private float spacing;

    // private List<GameObject> chips = new List<GameObject>();

    public void GenerateChips(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject chip = Instantiate(chipPrefab, transform);
            chip.transform.position = instantiatePosition + transform.right * i * spacing;
            chip.transform.rotation = Quaternion.Euler(0, 14.9f, -86f);
            // chips.Add(chip);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(instantiatePosition, 0.02f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(instantiatePosition, instantiatePosition + transform.right * 0.5f);
    }
}
