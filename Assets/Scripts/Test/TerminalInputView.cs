using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TerminalInputView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text inputText;

    [Header("Style")]
    [SerializeField] private string inputPrefix = "> ";
    [SerializeField] private bool showCursor = true;
    [SerializeField] private string cursorChar = "_";

    public void SetPrompt(string prompt)
    {
        if (promptText != null)
            promptText.text = prompt;
    }

    public void SetInput(string content)
    {
        if (inputText == null) return;

        string final = inputPrefix + content;
        if (showCursor)
            final += cursorChar;

        inputText.text = final;
    }

    public void SetInputNoCursor(string content)
    {
        if (inputText == null) return;
        inputText.text = inputPrefix + content;
    }

    public void ClearAll()
    {
        if (promptText != null)
            promptText.text = string.Empty;

        if (inputText != null)
            inputText.text = string.Empty;
    }

    public void ClearInputOnly()
    {
        if (inputText != null)
            inputText.text = string.Empty;
    }

    public void SetCursorVisible(bool visible)
    {
        showCursor = visible;
    }
}