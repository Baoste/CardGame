using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardHover : MonoBehaviour
{
    private SkillCardInstance instance;
    private SkillCardDraggable draggable;
    private float hoveredMoveDirY;

    private void Start()
    {
        instance = GetComponent<SkillCardInstance>();
        draggable = GetComponent<SkillCardDraggable>();
        hoveredMoveDirY = 0.4f;
    }

    void OnMouseEnter()
    {
        if (draggable.executed) return;
        if (draggable.IsDragging || ClientEffectContext.isExecutingSkillCard) return;
        transform.DOScale(Vector3.one * instance.localScaleFactor * 2.0f, 0.15f);
        
        Vector3 newPos = instance.originalPos;
        newPos.y = instance.originalPos.y + hoveredMoveDirY;
        transform.position = newPos;
        transform.DOMoveY(newPos.y + 0.1f, 0.5f).SetEase(Ease.OutCubic);
    }

    void OnMouseExit()
    {
        if (draggable.executed) return;
        if (draggable.IsDragging || ClientEffectContext.isExecutingSkillCard) return;
        transform.DOScale(Vector3.one * instance.localScaleFactor, 0.15f);

        Vector3 newPos = instance.originalPos;
        newPos.y = instance.originalPos.y + 0.1f;
        transform.position = newPos;
        transform.DOMoveY(instance.originalPos.y, 0.5f).SetEase(Ease.OutCubic);
    }
}
