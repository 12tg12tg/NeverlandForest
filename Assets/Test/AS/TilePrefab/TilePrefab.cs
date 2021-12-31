using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilePrefab : MonoBehaviour, IPointerClickHandler
{
    public Vector2 index;
    public Material[] material;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{index} Clicked!");
    }

    public void BlueLine()
    {
        //GetComponent<MeshRenderer>().materials += material;
    }

    public void RedLine()
    {

    }
}
