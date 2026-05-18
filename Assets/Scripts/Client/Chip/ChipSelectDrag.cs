using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChipSelectDrag : MonoBehaviour
{
    private Camera mainCamera;
    private ChipRaycastSelect chipRaycastSelect;

    private Plane dragPlane;

    private bool isDragging;

    private Vector3 dragStartMouseWorldPos;

    private readonly Dictionary<ChipController, Vector3> chipStartPositions = new();

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        chipRaycastSelect = GetComponent<ChipRaycastSelect>();

        Vector3 planeNormal = SceneViewManager.myChipView.transform.rotation * Vector3.up;
        dragPlane = new Plane(planeNormal, SceneViewManager.myChipView.transform.position);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginDragSelectedChips();
        }

        if (Input.GetMouseButton(0))
        {
            DragSelectedChips();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDragSelectedChips();
        }
    }

    private void BeginDragSelectedChips()
    {
        if (chipRaycastSelect == null)
            return;

        // 没有选中筹码，不需要拖拽
        if (chipRaycastSelect.SelectedChips.Count == 0)
            return;

        // 左键按下时，必须点到 ChipController
        if (!TryGetChipUnderMouse(out ChipController hitChip))
        {
            chipRaycastSelect.ClearSelection();
            return;
        }

        // 必须点到“已经被选中的 Chip”，否则清空选择
        if (!chipRaycastSelect.SelectedChips.Contains(hitChip))
        {
            chipRaycastSelect.ClearSelection();
            return;
        }

        if (!TryGetMouseWorldPosition(out dragStartMouseWorldPos))
            return;

        chipStartPositions.Clear();

        foreach (ChipController chip in chipRaycastSelect.SelectedChips)
        {
            if (chip == null)
                continue;

            chipStartPositions.Add(chip, chip.transform.position);

            if (chip.outlineControl != null)
            {
                chip.outlineControl.Enable = 1f;
                chip.outlineControl.OutlineColor = chip.outlineControl.defaultColor;
            }
        }

        isDragging = true;
    }

    private void DragSelectedChips()
    {
        if (!isDragging)
            return;

        if (!TryGetMouseWorldPosition(out Vector3 currentMouseWorldPos))
            return;

        Vector3 offset = currentMouseWorldPos - dragStartMouseWorldPos;

        foreach (KeyValuePair<ChipController, Vector3> pair in chipStartPositions)
        {
            ChipController chip = pair.Key;

            if (chip == null)
                continue;

            Vector3 startPos = pair.Value;
            chip.transform.position = startPos + offset;
        }

        UpdateAllChipOutlineByValidArea();
    }

    private void EndDragSelectedChips()
    {
        if (!isDragging)
            return;

        bool hasAnyInsideValidArea = false;

        // 先检查：是否至少有一个 Chip 在合法区域内
        foreach (KeyValuePair<ChipController, Vector3> pair in chipStartPositions)
        {
            ChipController chip = pair.Key;

            if (chip == null)
                continue;

            bool outside =
                SceneViewManager.myChipView != null &&
                SceneViewManager.myChipView.IsOutsideValidArea(chip.transform.position);

            if (!outside)
            {
                hasAnyInsideValidArea = true;
                break;
            }
        }

        // 再统一处理所有 Chip
        if (hasAnyInsideValidArea)
        {
            if (ClientGameState.playerSlot != ClientGameState.Instance.CurrentPlayerId)
            {
                Debug.Log("不是你的回合");
                chipRaycastSelect.ClearSelection();
            }
            //else if (SceneViewManager.opponentChipView.chipsInTray.Count < chipStartPositions.Count)
            //{
            //    Debug.Log("对方没有筹码了");
            //    chipRaycastSelect.ClearSelection();
            //}
            else
            {
                PlaceBetsCommand cmd = new PlaceBetsCommand { 
                    playerId = ClientGameState.playerSlot, 
                    instanceIds = chipStartPositions.Keys
                        .Where(chip => chip != null)
                        .Select(chip => chip.instanceId)
                        .ToArray()
                };
                ClientGameState.gateway.SendCommandServerRpc("PlaceBets", JsonConvert.SerializeObject(cmd));
                // chipRaycastSelect.PlaceSelection();
            }
        }
        else
        {
            chipRaycastSelect.ClearSelection();
        }

        isDragging = false;
        chipStartPositions.Clear();
    }

    private void UpdateAllChipOutlineByValidArea()
    {
        bool hasAnyInsideValidArea = false;

        foreach (KeyValuePair<ChipController, Vector3> pair in chipStartPositions)
        {
            ChipController chip = pair.Key;

            if (chip == null)
                continue;

            bool outside =
                SceneViewManager.myChipView != null &&
                SceneViewManager.myChipView.IsOutsideValidArea(chip.transform.position);

            if (!outside)
            {
                hasAnyInsideValidArea = true;
                break;
            }
        }

        foreach (KeyValuePair<ChipController, Vector3> pair in chipStartPositions)
        {
            ChipController chip = pair.Key;

            if (chip == null)
                continue;

            if (chip.outlineControl == null)
                continue;

            chip.outlineControl.OutlineColor = hasAnyInsideValidArea
                ? chip.outlineControl.defaultColor
                : chip.outlineControl.outAreaColor;
        }
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            worldPos = ray.GetPoint(enter);
            return true;
        }

        return false;
    }
    private bool TryGetChipUnderMouse(out ChipController chip)
    {
        chip = null;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            chip = hit.collider.GetComponentInParent<ChipController>();
            return chip != null;
        }

        return false;
    }
}
