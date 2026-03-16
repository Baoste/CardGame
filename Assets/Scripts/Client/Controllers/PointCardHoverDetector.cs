using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardHoverDetector : MonoBehaviour
{
    private int cardLayerMask;
    private PointCardHover currentHover;

    void Start()
    {
        cardLayerMask = LayerMask.GetMask("Card");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, cardLayerMask))
        {
            PointCardHover card = hit.collider.GetComponent<PointCardHover>();

            if (card != currentHover)
            {
                if (currentHover != null)
                {
                    currentHover.HidePoints();
                }
                currentHover = card;

                if (currentHover != null)
                {
                    currentHover.ShowPoints();
                }
            }
        }
        else
        {
            if (currentHover != null)
            {
                currentHover.HidePoints();
                currentHover = null;
                
            }
        }
    }
}
