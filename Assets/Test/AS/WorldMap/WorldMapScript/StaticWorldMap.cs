using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticWorldMap : MonoBehaviour
{
    public GameObject cube;
    public GameObject line;

    private WorldMapNode[,] maps;
    private int column = 10;
    private int row = 5;
    private float posX = 5f;
    private float posY = 15f;

    private void Awake()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.WorldMapPlayerData);
        Load();
        PaintLink();
    }

    public void Load()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.WorldMapNode);
        var loadData = Vars.UserData.WorldMapNodeStruct;

        maps = new WorldMapNode[row, column];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var index = new Vector2(j, i);
                for (int k = 0; k < loadData.Count; k++)
                {
                    if (loadData[k].index.Equals(index))
                    {
                        InitNode(out maps[j, i], index);
                    }
                }
            }
        }

        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var index = new Vector2(j, i);
                for (int k = 0; k < loadData.Count; k++)
                {
                    if (loadData[k].index.Equals(index))
                    {
                        var children = loadData[k].children;
                        var parent = loadData[k].parent;
                        for (int h = 0; h < children.Count; h++)
                        {
                            maps[j, i].Children.Add(maps[(int)children[h].x, (int)children[h].y]);
                        }
                        for (int h = 0; h < parent.Count; h++)
                        {
                            maps[j, i].Parent.Add(maps[(int)parent[h].x, (int)parent[h].y]);
                        }
                    }
                }
            }
        }
    }

    public void PaintLink()
    {
        var lines = new GameObject();
        lines.transform.SetParent(transform);
        DefineLayer(lines);
        var curIndex = Vars.UserData.WorldMapData.currentIndex;
        var goalIndex = Vars.UserData.WorldMapData.goalIndex;
        for (int i = 0; i < column - 1; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (maps[j, i] == null)
                    continue;

                var lineGo = Instantiate(line, lines.transform);
                DefineLayer(lineGo);
                var lineRender = lineGo.GetComponent<LineRenderer>();
                lineRender.startWidth = lineRender.endWidth = 0.1f;
                if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[0].index.Equals(goalIndex))
                {
                    //lineRender.material.color = Color.yellow;
                }
                else
                {
                    lineRender.material.color = Color.white;
                }
                lineRender.SetPosition(0, maps[j, i].transform.position);
                lineRender.SetPosition(1, maps[j, i].Children[0].transform.position);

                if (maps[j, i].Children.Count >= 2)
                {
                    var lineGoSecond = Instantiate(line, lines.transform);
                    DefineLayer(lineGoSecond);
                    var lineRenderSecond = lineGoSecond.GetComponent<LineRenderer>();
                    lineRenderSecond.startWidth = lineRenderSecond.endWidth = 0.1f;
                    if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[1].index.Equals(goalIndex))
                    {
                        //lineRenderSecond.material.color = Color.yellow;
                    }
                    else
                    {
                        lineRenderSecond.material.color = Color.white;
                    }
                    lineRenderSecond.SetPosition(0, maps[j, i].transform.position);
                    lineRenderSecond.SetPosition(1, maps[j, i].Children[1].transform.position);
                }
                if (maps[j, i].Children.Count >= 3)
                {
                    var lineGoThird = Instantiate(line, lines.transform);
                    DefineLayer(lineGoThird);
                    var lineRenderThird = lineGoThird.GetComponent<LineRenderer>();
                    lineRenderThird.startWidth = lineRenderThird.endWidth = 0.1f;
                    if (maps[j, i].index.Equals(curIndex) && maps[j, i].Children[2].index.Equals(goalIndex))
                    {
                        //lineRenderThird.material.color = Color.yellow;
                    }
                    else
                    {
                        lineRenderThird.material.color = Color.white;
                    }
                    lineRenderThird.SetPosition(0, maps[j, i].transform.position);
                    lineRenderThird.SetPosition(1, maps[j, i].Children[2].transform.position);
                }
            }
        }
    }

    private void InitNode(out WorldMapNode node, Vector2 index)
    {
        var go = Instantiate(cube, new Vector3(index.y * posY - 200f, 100f + index.x * posX, 0f), Quaternion.identity);
        DefineLayer(go);
        go.transform.SetParent(gameObject.transform);
        node = go.AddComponent<WorldMapNode>();
        node.difficulty = (Difficulty)Random.Range(0, 3);
        node.index = index;
    }

    private void DefineLayer(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("WorldMap");
    }
}
