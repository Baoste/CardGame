using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteractionController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ComputerUIController uiController;
    [SerializeField] private GameObject interactHint;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour[] playerControlScripts;

    [Header("Cursor")]
    [SerializeField] private bool showCursorWhenUsingComputer = true;
    [SerializeField] private CursorLockMode enterComputerCursorLockMode = CursorLockMode.None;
    [SerializeField] private CursorLockMode exitComputerCursorLockMode = CursorLockMode.Locked;
    [SerializeField] private bool hideCursorWhenExitComputer = true;

    [Header("Debug State")]
    [SerializeField] private bool playerInRange;
    [SerializeField] private bool isUsingComputer;

    public ShotPlayer shotPlayer;

    public bool PlayerInRange => playerInRange;
    public bool IsUsingComputer => isUsingComputer;

    private void Start()
    {
        RefreshInteractHint();
    }

    private void Update()
    {
        if (!isUsingComputer)
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Press E -> EnterComputer");
                EnterComputer();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Press Esc -> ExitComputer");
                ExitComputer();
            }
        }
    }

    public void SetPlayerInRange(bool value)
    {
        playerInRange = value;
        RefreshInteractHint();
    }

    public void EnterComputer()
    {
        if (isUsingComputer)
            return;

        shotPlayer.PlayShot(2); // 切换到电脑交互摄像机

        isUsingComputer = true;

        SetPlayerControlEnabled(false);
        //ApplyEnterCursorState();

        if (uiController != null)
        {
            uiController.OpenComputer();
        }

        RefreshInteractHint();
    }

    public void ExitComputer()
    {
        if (!isUsingComputer)
            return;

        shotPlayer.PlayShot(1);

        isUsingComputer = false;

        SetPlayerControlEnabled(true);
        //ApplyExitCursorState();

        if (uiController != null)
        {
            uiController.CloseComputer();
        }

        RefreshInteractHint();
    }

    private void SetPlayerControlEnabled(bool enabled)
    {
        if (playerControlScripts == null)
            return;

        for (int i = 0; i < playerControlScripts.Length; i++)
        {
            if (playerControlScripts[i] != null)
            {
                playerControlScripts[i].enabled = enabled;
            }
        }
    }

    private void ApplyEnterCursorState()
    {
        Cursor.lockState = enterComputerCursorLockMode;
        Cursor.visible = showCursorWhenUsingComputer;
    }

    private void ApplyExitCursorState()
    {
        Cursor.lockState = exitComputerCursorLockMode;
        Cursor.visible = !hideCursorWhenExitComputer;
    }

    private void RefreshInteractHint()
    {
        if (interactHint == null)
            return;

        bool shouldShow = playerInRange && !isUsingComputer;
        interactHint.SetActive(shouldShow);
    }
}