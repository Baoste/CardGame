using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalTextController : MonoBehaviour
{
    [Header("Lines")]
    [SerializeField] private TMP_Text[] lineTexts;

    [Header("Typing")]
    [SerializeField] private float charInterval = 0f;
    [SerializeField] private float lineStayDuration = 0f;
    [SerializeField] private bool useBlinkCursor = true;
    [SerializeField] private string cursorChar = "_";

    private readonly List<string> visibleLines = new();
    private Coroutine playCoroutine;
    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    public enum TerminalLineMode
    {
        Typewriter,
        Instant
    }

    [System.Serializable]
    public struct TerminalLine
    {
        public string text;
        public TerminalLineMode mode;

        public TerminalLine(string text, TerminalLineMode mode)
        {
            this.text = text;
            this.mode = mode;
        }
    }

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

    public void PlayLines(IEnumerable<TerminalLine> lines)
    {
        if (playCoroutine != null)
            StopCoroutine(playCoroutine);

        playCoroutine = StartCoroutine(PlayTerminalLinesRoutine(lines));
    }

    private IEnumerator PlayTerminalLinesRoutine(IEnumerable<TerminalLine> lines)
    {
        isPlaying = true;

        foreach (TerminalLine line in lines)
        {
            yield return PlayOneLine(line.text, line.mode);
        }

        isPlaying = false;
        playCoroutine = null;
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
            yield return PlayOneLine(line, TerminalLineMode.Typewriter);
        }

        isPlaying = false;
        playCoroutine = null;
    }

    private IEnumerator PlaySingleLineRoutine(string line)
    {
        isPlaying = true;
        yield return PlayOneLine(line, TerminalLineMode.Typewriter);
        isPlaying = false;
        playCoroutine = null;
    }

    //private IEnumerator PlayLinesRoutine(IEnumerable<TerminalLine> lines)
    //{
    //    isPlaying = true;

    //    foreach (TerminalLine line in lines)
    //    {
    //        yield return PlayOneLine(line.text, line.mode);
    //    }

    //    isPlaying = false;
    //    playCoroutine = null;
    //}

    //private IEnumerator PlaySingleLineRoutine(TerminalLine line)
    //{
    //    isPlaying = true;

    //    yield return PlayOneLine(line.text, line.mode);

    //    isPlaying = false;
    //    playCoroutine = null;
    //}

    private IEnumerator PlayOneLine(string fullLine, TerminalLineMode mode)
    {
        PushLine(string.Empty);

        if (mode == TerminalLineMode.Instant || charInterval <= 0f)
        {
            visibleLines[visibleLines.Count - 1] = fullLine;
            RefreshView(-1, null, false);

            if (lineStayDuration > 0f)
                yield return new WaitForSeconds(lineStayDuration);

            yield break;
        }

        for (int i = 0; i <= fullLine.Length; i++)
        {
            string current = fullLine.Substring(0, i);
            RefreshView(visibleLines.Count - 1, current, useBlinkCursor);
            yield return new WaitForSeconds(charInterval);
        }

        visibleLines[visibleLines.Count - 1] = fullLine;
        RefreshView(-1, null, false);

        if (lineStayDuration > 0f)
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