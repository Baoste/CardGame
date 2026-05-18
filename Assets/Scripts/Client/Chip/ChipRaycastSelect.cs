using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipRaycastSelect : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private LayerMask chipLayerMask;

    private readonly List<ChipController> selectedChips = new();

    // 本次右键按住期间，已经被切换过的 Chip
    private readonly HashSet<ChipController> toggledThisHold = new();

    public IReadOnlyList<ChipController> SelectedChips => selectedChips;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        // 右键刚按下时，清空本次按住记录
        if (Input.GetMouseButtonDown(1))
        {
            toggledThisHold.Clear();
        }

        // 右键按住期间，持续检测
        if (Input.GetMouseButton(1))
        {
            TryToggleChipUnderMouse();
        }

        // 右键松开后，清空本次按住记录
        if (Input.GetMouseButtonUp(1))
        {
            toggledThisHold.Clear();
        }
    }

    private void TryToggleChipUnderMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, chipLayerMask))
        {
            ChipController chip = hit.collider.GetComponentInParent<ChipController>();

            if (chip == null)
                return;

            // 同一次右键按住期间，同一个 Chip 只切换一次
            if (toggledThisHold.Contains(chip))
                return;

            toggledThisHold.Add(chip);

            ToggleChip(chip);
        }
    }

    private void ToggleChip(ChipController chip)
    {
        if (selectedChips.Contains(chip))
        {
            selectedChips.Remove(chip);
            chip.stateMachine.ChangeState(chip.inTrayState);
        }
        else
        {
            selectedChips.Add(chip);
            chip.stateMachine.ChangeState(chip.selectedState);
        }
    }

    public void ClearSelection()
    {
        foreach (ChipController chip in selectedChips)
        {
            if (chip != null)
                chip.stateMachine.ChangeState(chip.inTrayState);
        }

        selectedChips.Clear();
        toggledThisHold.Clear();
    }

    public void PlaceSelection()
    {
        foreach (ChipController chip in selectedChips)
        {
            if (chip != null)
                chip.stateMachine.ChangeState(chip.placedState);
        }

        selectedChips.Clear();
        toggledThisHold.Clear();
    }

    public void ClearList()
    {
        selectedChips.Clear();
        toggledThisHold.Clear();
    }
}
