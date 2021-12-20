using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode<T>
{
    public T Data { get; set; }
    public Vector2 pos;
    public List<MapNode<T>> right; // ������ �ΰ� �̻��� �� �� �ֱ� ������
    public List<MapNode<T>> left;

    public MapNode(T data)
    {
        this.Data = data;
    }
}