using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteractTrigger : MonoBehaviour
{
    [SerializeField] private ComputerInteractionController interactionController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (interactionController != null)
        {
            interactionController.SetPlayerInRange(true);
        }

        Debug.Log("Player entered computer interact trigger.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (interactionController != null)
        {
            interactionController.SetPlayerInRange(false);
        }

        Debug.Log("Player exited computer interact trigger.");
    }
}