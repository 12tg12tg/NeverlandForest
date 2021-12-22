using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class MapNode : MonoBehaviour, IPointerClickHandler
{
    public List<MapNode> children = new List<MapNode>();
    public List<MapNode> parent = new List<MapNode>();
    public List<MapNode> Children { get => children; set => children = value; }
    public List<MapNode> Parent { get => parent; set => parent = value; }

    public Vector2 index;
    public int level;

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            Debug.Log(Children[i].index);
        }
    }
}
