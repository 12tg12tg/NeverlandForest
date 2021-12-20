using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour
{
    public List<StageNode> downVertice = new List<StageNode>();
    public List<StageNode> upVertice = new List<StageNode>();
    public Vector2 index = Vector2.zero;
    
    public Vector2 Index { get => index; set => index = value; }
    public int Row { get => (int)index.x; set => index.x = value; }
    public int Col { get => (int)index.y; }

}
