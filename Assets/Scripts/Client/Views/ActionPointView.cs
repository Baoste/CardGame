using System.Collections.Generic;
using UnityEngine;

public class ActionPointView : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    private Stack<Renderer> offStack = new Stack<Renderer>();
    private Stack<Renderer> onStack = new Stack<Renderer>();
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.gray;

    private void Awake()
    {
        // 给每个子物体创建材质实例，避免共享
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = new Material(renderers[i].sharedMaterial);
            renderers[i].material.color = offColor;
            offStack.Push(renderers[i]);
        }
    }

    public void ResetPoint()
    {
        while (onStack.Count > 0)
        {
            Renderer r = onStack.Pop();
            r.material.color = offColor;
            offStack.Push(r);
        }
    }

    public void AddPoint(int count)
    {
        while (count > 0 && offStack.Count > 0)
        {
            Renderer r = offStack.Pop();
            r.material.color = onColor;
            r.material.EnableKeyword("_EMISSION");
            onStack.Push(r);
            count--;
        }
    }

    public void SpendPoint(int count)
    {
        while (count > 0 && onStack.Count > 0)
        {
            Renderer r = onStack.Pop();
            r.material.color = offColor;
            r.material.DisableKeyword("_EMISSION");
            offStack.Push(r);
            count--;
        }
    }
}