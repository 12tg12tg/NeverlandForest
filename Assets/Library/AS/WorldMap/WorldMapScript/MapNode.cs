using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode<T>
{
    public T Data { get; set; }
    public Vector2 pos;
    public List<MapNode<T>> right; // 갈래가 두개 이상이 될 수 있기 때문에
    public List<MapNode<T>> left;

    public MapNode(T data)
    {
        this.Data = data;
    }
}