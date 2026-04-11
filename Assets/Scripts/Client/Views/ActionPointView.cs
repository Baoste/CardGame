using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointView : MonoBehaviour
{
    [SerializeField] private List<Light> lights = new List<Light>();
    
    private Stack<Light> lightsOff = new Stack<Light>();
    private Stack<Light> lightsOn = new Stack<Light>();

    private void Start()
    {
        foreach (Light light in lights)
        {
            light.intensity = 0;
            lightsOff.Push(light);
        }
    }

    public void ResetPoint()
    {
        while (lightsOn.Count > 0)
        {
            Light light = lightsOn.Pop();
            light.intensity = 0;
            lightsOff.Push(light);
        }
    }

    public void AddPoint(int count)
    {
        while (count > 0 && lightsOff.Count > 0)
        {
            Light light = lightsOff.Pop();
            light.intensity = 0.1f;
            lightsOn.Push(light);
            count--;
        }
    }

    public void SpendPoint(int count)
    {
        while (count > 0 && lightsOn.Count > 0)
        {
            Light light = lightsOn.Pop();
            light.intensity = 0;
            lightsOff.Push(light);
            count--;
        }
    }
}
