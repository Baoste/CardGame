using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ComputerScreenState
{
    None,
    Boot,
    MainMenu,
    InputRoomCode,
    MatchingRandom,
    MatchingByRoomCode,
    Result
}

public class ComputerUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TerminalTextController terminalLog;
    [SerializeField] private TerminalInputView inputView;
    [SerializeField] private ComputerInteractionController interactionController;

    [Header("Config")]
    [SerializeField] private int roomCodeMaxLength = 6;
    [SerializeField] private bool onlyAllowDigits = true;

    [Header("Debug State")]
    [SerializeField] private ComputerScreenState currentState = ComputerScreenState.None;
    [SerializeField] private int mainMenuIndex = 0;
    [SerializeField] private string currentRoomCode = "";

    public System.Action<string> OnJoinRoomRequested;
    public System.Action OnRandomMatchRequested;
    private bool inputLocked;

    public SceneSwitcher sceneSwitcher;


    private readonly string[] mainMenuOptions =
    {
        "Random Match",
        "Tutorial",
        // "Join by Room Code",
        "Exit Terminal"
    };

    private Coroutine bootRoutine;
    private Coroutine matchingRoutine;

    public ComputerScreenState CurrentState => currentState;

    // 开电脑
    public void OpenComputer()
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);

        StopAllLocalRoutines();

        currentRoomCode = string.Empty;
        mainMenuIndex = 0;

        if (terminalLog != null)
            terminalLog.ClearAll();

        if (inputView != null)
            inputView.ClearAll();

        EnterBootState();
    }

    // 关电脑
    public void CloseComputer()
    {
        StopAllLocalRoutines();

        currentState = ComputerScreenState.None;
        currentRoomCode = string.Empty;
        mainMenuIndex = 0;

        if (terminalLog != null)
            terminalLog.ClearAll();

        if (inputView != null)
            inputView.ClearAll();

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    // Update
    public void Update()
    {
        if (rootPanel == null || !rootPanel.activeSelf)
            return;

        if (inputLocked)
            return;

        switch (currentState)
        {
            case ComputerScreenState.Boot:
                UpdateBootState();
                break;

            case ComputerScreenState.MainMenu:
                UpdateMainMenuState();
                break;

            case ComputerScreenState.InputRoomCode:
                UpdateInputRoomCodeState();
                break;

            case ComputerScreenState.MatchingRandom:
                
            case ComputerScreenState.MatchingByRoomCode:
                UpdateMatchingState();
                break;

            case ComputerScreenState.Result:
                UpdateResultState();
                break;
        }
    }

    #region State Enter

    private void EnterBootState()
    {
        currentState = ComputerScreenState.Boot;

        if (inputView != null)
        {
            inputView.SetPrompt("> System");
            inputView.SetInputNoCursor("BOOTING");
        }

        if(bootRoutine != null)
        {
            StopCoroutine(bootRoutine);
        }

        bootRoutine = StartCoroutine(BootSequenceRoutine());
    }

    private void EnterMainMenuState()
    {
        //StopMatchingRoutineIfNeed();

        currentState = ComputerScreenState.MainMenu;
        currentState = ComputerScreenState.MainMenu;
        mainMenuIndex = 0;

        if(terminalLog!= null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new TerminalTextController.TerminalLine[]
            {
                new TerminalTextController.TerminalLine("MATCH SYSTEM OS READY.", TerminalTextController.TerminalLineMode.Instant),
                new TerminalTextController.TerminalLine("Navigation: W/S or Up/Down", TerminalTextController.TerminalLineMode.Instant),
                new TerminalTextController.TerminalLine("Confirm: Enter", TerminalTextController.TerminalLineMode.Instant),
                new TerminalTextController.TerminalLine("Exit: Esc", TerminalTextController.TerminalLineMode.Instant)
            });
        }

        RefreshMainMenuInputView();
    }

    //private void EnterInputRoomCodeState()
    //{
    //    StopMatchingRoutineIfNeed();

    //    currentState = ComputerScreenState.InputRoomCode;
    //    currentRoomCode = string.Empty;

    //    if (terminalLog != null)
    //    {
    //        terminalLog.ClearAll();
    //        terminalLog.PlayLines(new string[]
    //        {
    //            "Manual room connection selected.",
    //            "Please enter room code."
    //        });
    //    }

    //    RefreshRoomCodeInputView();
    //}

    private void EnterInputRoomCodeState()
    {
        StopMatchingRoutineIfNeed();

        currentState = ComputerScreenState.InputRoomCode;
        currentRoomCode = string.Empty;

        if (terminalLog != null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new string[]
            {
                "Select Tutorial."
            });
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("Tutorial");
        // RefreshRoomCodeInputView();
    }

    private void EnterMatchingRandomState()
    {
        StopMatchingRoutineIfNeed();
        currentState = ComputerScreenState.MatchingRandom;

        if (terminalLog != null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new string[]
            {
                "Random match request received.",
                "Searching for available session..."
            });
        }

        if (inputView != null)
        {
            inputView.SetPrompt("> Status");
            inputView.SetInputNoCursor("MATCHING");
        }

        OnRandomMatchRequested?.Invoke();
        matchingRoutine = StartCoroutine(FakeMatchingRoutine(true, null));
    }

    private void EnterMatchingByRoomCodeState()
    {
        //StopMatchingRoutineIfNeed();

        currentState = ComputerScreenState.MatchingByRoomCode;
        inputLocked = true;

        if (terminalLog != null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new string[]
            {
                "Sending room join request...",
                "Room Code: " + currentRoomCode,
                "Waiting for server response..."
            });
        }
        if (inputView != null)
        {
            inputView.SetPrompt("> Room Code");
            inputView.SetInputNoCursor(currentRoomCode);
        }

        matchingRoutine = StartCoroutine(FakeMatchingRoutine(false, currentRoomCode));
    }

    private void EnterResultState(string resultTitle, string resultBody)
    {
        StopMatchingRoutineIfNeed();

        currentState = ComputerScreenState.Result;

        if (terminalLog != null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new string[]
            {
                resultTitle,
                resultBody,
                "Press Enter to return to menu."
            });
        }

        if (inputView != null)
        {
            inputView.SetPrompt("> Result");
            inputView.SetInputNoCursor("COMPLETE");
        }
    }
    #endregion

    #region State Update

    private void UpdateBootState()
    {
        if (Input.anyKeyDown)
        {
            EnterMainMenuState();
        }
    }

    private void UpdateMainMenuState()
    {
        if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            mainMenuIndex--;
            if (mainMenuIndex < 0)
                mainMenuIndex = mainMenuOptions.Length - 1;

            RefreshMainMenuInputView();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            mainMenuIndex++;
            if (mainMenuIndex >= mainMenuOptions.Length)
                mainMenuIndex = 0;

            RefreshMainMenuInputView();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmMainMenuOption();
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    ExitComputerCompletely();
        //}
    }

    private void UpdateInputRoomCodeState()
    {
        HandleRoomCodeTyping();

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!string.IsNullOrEmpty(currentRoomCode))
            {
                currentRoomCode = currentRoomCode.Substring(0, currentRoomCode.Length - 1);
                RefreshRoomCodeInputView();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmJoinRoomByCode();
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    EnterMainMenuState();
        //}
    }

    private void UpdateMatchingState()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (terminalLog != null)
        //    {
        //        terminalLog.PlaySingleLine("Operation cancelled by user.");
        //    }

        //    EnterMainMenuState();
        //}
    }

    private void UpdateResultState()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape))
        {
            EnterMainMenuState();
        }
    }

    #endregion

    #region Input Refresh

    private void RefreshMainMenuInputView()
    {
        if (inputView == null) return;

        inputView.SetPrompt("Main Menu");
        inputView.SetInputNoCursor(BuildMenuDisplay());
    }

    private void RefreshRoomCodeInputView()
    {
        if (inputView == null) return;

        inputView.SetPrompt("> Enter Room Code");
        inputView.SetInput(currentRoomCode);
    }

    private string BuildMenuDisplay()
    {
        return mainMenuOptions[mainMenuIndex];
    }

    #endregion

    #region Confirm Logic

    private void ConfirmMainMenuOption()
    {
        switch (mainMenuIndex)
        {
            case 0:
                EnterMatchingRandomState();
                break;

            case 1:
                EnterInputRoomCodeState();
                break;

            case 2:
                ExitComputerCompletely();
                break;
        }
    }

    private void ConfirmJoinRoomByCode()
    {
        if (string.IsNullOrEmpty(currentRoomCode))
        {
            if (terminalLog != null)
                terminalLog.PlaySingleLine("Room code cannot be empty.");

            return;
        }

        EnterMatchingByRoomCodeState();

        // 发送Roomcode到服务器
        OnJoinRoomRequested?.Invoke(currentRoomCode);
    }

    #endregion

    #region Typing

    private void HandleRoomCodeTyping()
    {
        string input = Input.inputString;
        if (string.IsNullOrEmpty(input))
            return;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '\b' || c == '\n' || c == '\r')
                continue;

            if (currentRoomCode.Length >= roomCodeMaxLength)
                break;

            if (onlyAllowDigits)
            {
                if (char.IsDigit(c))
                    currentRoomCode += c;
            }
            else
            {
                if (char.IsLetterOrDigit(c))
                    currentRoomCode += char.ToUpperInvariant(c);
            }
        }

        RefreshRoomCodeInputView();
    }

    #endregion

    #region Routine

    private IEnumerator BootSequenceRoutine()
    {
        if (terminalLog != null)
        {
            terminalLog.ClearAll();
            terminalLog.PlayLines(new string[]
            {
                "MATCH SYSTEM OS",
                "Initializing terminal modules...",
                "Checking network status...",
                "Loading matchmaking service...",
                "System ready.",
                "Press any key to continue."
            });
        }

        yield return null;
        bootRoutine = null;
    }

    private IEnumerator FakeMatchingRoutine(bool isRandomMatch, string roomCode)
    {
        yield return new WaitForSeconds(1.0f);

        if (terminalLog != null)
            terminalLog.PlaySingleLine("Authorizing user session...");

        yield return new WaitForSeconds(1.0f);

        if (isRandomMatch)
        {
            if (terminalLog != null)
                terminalLog.PlaySingleLine("Candidate session found.");

            sceneSwitcher.SwitchScene();
        }
        else
        {
            if (terminalLog != null)
                terminalLog.PlaySingleLine("Room " + roomCode + " verified.");
        }

        yield return new WaitForSeconds(1.0f);

        bool success = true;

        if (success)
        {
            if (isRandomMatch)
            {
                EnterResultState("MATCH SUCCESS", "Connected to random session.");
            }
            else
            {
                EnterResultState("ROOM JOIN SUCCESS", "Connected to room " + roomCode + ".");
            }
        }
        else
        {
            if (isRandomMatch)
            {
                EnterResultState("MATCH FAILED", "No available session found.");
            }
            else
            {
                EnterResultState("ROOM JOIN FAILED", "Room " + roomCode + " was not found.");
            }
        }

        matchingRoutine = null;
    }

    #endregion

    #region Helper

    private void ExitComputerCompletely()
    {
        if (interactionController != null)
        {
            interactionController.ExitComputer();
        }
        else
        {
            CloseComputer();
        }
    }

    private void StopAllLocalRoutines()
    {
        if (bootRoutine != null)
        {
            StopCoroutine(bootRoutine);
            bootRoutine = null;
        }

        if (matchingRoutine != null)
        {
            StopCoroutine(matchingRoutine);
            matchingRoutine = null;
        }
    }

    private void StopMatchingRoutineIfNeed()
    {
        if (matchingRoutine != null)
        {
            StopCoroutine(matchingRoutine);
            matchingRoutine = null;
        }
    }

    #endregion

    public void OnMatchSuccess(string message)
    {
        inputLocked = false;

        EnterResultState("MATCH SUCCESS", message);
    }

    public void OnMatchFailed(string message)
    {
        inputLocked = false;

        EnterResultState("MATCH FAILED", message);
    }
}
