using System.Collections.Generic;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
    static readonly Queue<string> lines = new Queue<string>();

    void OnEnable() => Application.logMessageReceived += Handle;
    void OnDisable() => Application.logMessageReceived -= Handle;

    void Handle(string condition, string stackTrace, LogType type)
    {
        lines.Enqueue($"[{type}] {condition}");
        while (lines.Count > 15) lines.Dequeue();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 900, 600));
        foreach (var l in lines) GUILayout.Label(l);
        GUILayout.EndArea();
    }
}