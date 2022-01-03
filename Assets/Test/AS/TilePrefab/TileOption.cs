using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileOption : MonoBehaviour, IPointerClickHandler
{
    public MeshRenderer meshRenderer;
    public Vector2 index;
    public bool isBlue = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBlue)
        {
            Blue();
            isBlue = false;
        }
        else
        {
            Red();
            isBlue = true;
        }
    }

    public void Blue()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void Red()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
