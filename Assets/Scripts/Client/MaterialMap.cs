using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMap : MonoBehaviour
{
    [SerializeField] private List<MaterialEntry> materialList;
    private Dictionary<CardState, Material[]> materialDict;

    private void Awake()
    {
        materialDict = new Dictionary<CardState, Material[]>();
        foreach (var entry in materialList)
        {
            materialDict[entry.state] = entry.materials;
        }
    }

    public Material[] Get(CardState state)
    {
        if (materialDict.TryGetValue(state, out Material[] materials))
        {
            return materials;
        }
        else
        {
            Debug.LogWarning($"Material for state {state} not found.");
            return null;
        }
    }
}
