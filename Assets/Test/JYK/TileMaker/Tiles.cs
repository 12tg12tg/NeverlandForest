using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{index} Clicked!");
        ren.enabled = true;
    }
}
