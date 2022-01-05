using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Difficulty
{
    easy,
    normal,
    hard,
}

[Serializable]
public class WorldMapNode : MonoBehaviour, IPointerClickHandler
{
    public event Action<WorldMapNode> OnClick; // 얘는 대문자로 사용해야 한다고 함

    public List<WorldMapNode> Children { get; set; } = new List<WorldMapNode>();
    public List<WorldMapNode> Parent { get; set; } = new List<WorldMapNode>();

    public Difficulty difficulty;
    public Vector2 index;
    public int level;

    public void OnPointerClick(PointerEventData eventData) => OnClick(this);
}