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
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (interactionController != null)
        {
            interactionController.SetPlayerInRange(false);
        }
    }
}