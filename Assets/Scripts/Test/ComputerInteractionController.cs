using FishNet.Demo.AdditiveScenes;
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
    [SerializeField] private GameObject player;
    private Rigidbody playerRb;
    private RigidbodyConstraints originalConstraints;

    [Header("Cursor")]
    [SerializeField] private bool showCursorWhenUsingComputer = true;
    [SerializeField] private CursorLockMode enterComputerCursorLockMode = CursorLockMode.None;
    [SerializeField] private CursorLockMode exitComputerCursorLockMode = CursorLockMode.Locked;
    [SerializeField] private bool hideCursorWhenExitComputer = true;

    [Header("Debug State")]
    [SerializeField] private bool playerInRange;
    [SerializeField] private bool isUsingComputer;

    public ShotPlayer shotPlayer;
    public GameObject screenMesh;

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
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        Debug.Log("Press Esc -> ExitComputer");
        //        ExitComputer();
        //    }
        //}
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

        playerRb = player.GetComponent<Rigidbody>();
        originalConstraints = playerRb.constraints;

        shotPlayer.PlayShot(3); // 切换到电脑交互摄像机

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.constraints = RigidbodyConstraints.FreezeAll;

        screenMesh.GetComponent<Renderer>().material.SetInt("_isStandby", 0); // 切换到正常屏幕材质

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

        shotPlayer.PlayShot(2);

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.constraints = originalConstraints;

        screenMesh.GetComponent<Renderer>().material.SetInt("_isStandby", 1); // 切换到待机屏幕材质

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