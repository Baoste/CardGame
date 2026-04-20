using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalTextController : MonoBehaviour
{
    [Header("Lines")]
    [SerializeField] private TMP_Text[] lineTexts;

    [Header("Typing")]
    [SerializeField] private float charInterval = 0.03f;
    [SerializeField] private float lineStayDuration = 0.4f;
    [SerializeField] private bool useBlinkCursor = true;
    [SerializeField] private string cursorChar = "_";

    private readonly List<string> visibleLines = new();
    private Coroutine playCoroutine;
    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        ClearAll();
    }

    public void ClearAll()
    {
        visibleLines.Clear();

        if (lineTexts == null) return;

        for (int i = 0; i < lineTexts.Length; i++)
        {
            if (lineTexts[i] != null)
                lineTexts[i].text = string.Empty;
        }
    }

    public void PlayLines(IEnumerable<string> lines)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }

        playCoroutine = StartCoroutine(PlayLinesRoutine(lines));
    }

    public void PlaySingleLine(string line)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }

        playCoroutine = StartCoroutine(PlaySingleLineRoutine(line));
    }

    public void AppendLineImmediate(string line)
    {
        PushLine(line);
        RefreshView(-1, null, false);
    }

    private IEnumerator PlayLinesRoutine(IEnumerable<string> lines)
    {
        isPlaying = true;

        foreach (string line in lines)
        {
            yield return PlayOneLine(line);
        }

        isPlaying = false;
        playCoroutine = null;
    }

    private IEnumerator PlaySingleLineRoutine(string line)
    {
        isPlaying = true;
        yield return PlayOneLine(line);
        isPlaying = false;
        playCoroutine = null;
    }

    private IEnumerator PlayOneLine(string fullLine)
    {
        PushLine(string.Empty);

        for (int i = 0; i <= fullLine.Length; i++)
        {
            string current = fullLine.Substring(0, i);
            RefreshView(visibleLines.Count - 1, current, useBlinkCursor);
            yield return new WaitForSeconds(charInterval);
        }

        visibleLines[visibleLines.Count - 1] = fullLine;
        RefreshView(-1, null, false);

        yield return new WaitForSeconds(lineStayDuration);
    }

    private void PushLine(string line)
    {
        visibleLines.Add(line);

        int maxCount = lineTexts != null ? lineTexts.Length : 0;
        while (visibleLines.Count > maxCount)
        {
            visibleLines.RemoveAt(0);
        }
    }

    private void RefreshView(int typingLineIndex, string typingContent, bool showCursor)
    {
        if (lineTexts == null) return;

        for (int i = 0; i < lineTexts.Length; i++)
        {
            if (lineTexts[i] == null) continue;
            lineTexts[i].text = string.Empty;
        }

        int start = Mathf.Max(0, visibleLines.Count - lineTexts.Length);

        for (int i = 0; i < lineTexts.Length; i++)
        {
            int sourceIndex = start + i;
            if (sourceIndex >= visibleLines.Count) continue;

            string content = visibleLines[sourceIndex];

            if (sourceIndex == typingLineIndex && typingContent != null)
            {
                content = typingContent;
                if (showCursor)
                    content += cursorChar;
            }

            lineTexts[i].text = content;
        }
    }
}