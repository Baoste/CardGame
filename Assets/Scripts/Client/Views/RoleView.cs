using Cinemachine;
using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class RoleView : MonoBehaviour
{
    [SerializeField] private Renderer MyDealer;
    [SerializeField] private Renderer OpDealer;
    [SerializeField] private VisualEffect My_DissolutionVFX;
    [SerializeField] private VisualEffect OP_DissolutionVFX;

    private MaterialPropertyBlock block;

    private void Start()
    {
        block = new MaterialPropertyBlock();
        My_DissolutionVFX.Stop();
        OP_DissolutionVFX.Stop();
    }

    public void ShowRole(int dealerId)
    {
        if (dealerId == ClientGameState.playerSlot)
        {
            ApplyDissolution(MyDealer, My_DissolutionVFX, 0);
        }
        else
        {
            ApplyDissolution(OpDealer, OP_DissolutionVFX, 0);
        }
    }

    private void ApplyDissolution(Renderer renderer, VisualEffect vfx, float targetValue)
    {
        vfx.Play();

        float current = 1.2f;
        DOTween.To(
            () => current,
            x =>
            {
                current = x;

                renderer.GetPropertyBlock(block);
                block.SetFloat("_DissolutionStrength", current);
                renderer.SetPropertyBlock(block);
            },
            targetValue,
            1f
        );
    }

    public IEnumerator ShowWin(int winnerId)
    {
        if (winnerId == ClientGameState.playerSlot)
        {
            // TODO: show win effect
        }
        else
        {
            // TODO: show lose effect
        }

        ApplyDissolution(MyDealer, My_DissolutionVFX, 1.2f);
        ApplyDissolution(OpDealer, OP_DissolutionVFX, 1.2f);
        yield break;
    }
}
