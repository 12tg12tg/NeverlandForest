using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class WorldMapNode : MonoBehaviour, IPointerClickHandler
{
    public event UnityAction<WorldMapNode> OnClick; // ��� �빮�ڷ� ����ؾ� �Ѵٰ� ��
    public List<WorldMapNode> Children { get; set; } = new List<WorldMapNode>();
    public List<WorldMapNode> Parent { get; set; } = new List<WorldMapNode>();

    public Vector2 index;
    public int level;

    public void OnPointerClick(PointerEventData eventData) => OnClick(this);
}