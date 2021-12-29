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
    public WorldMapPlayer player;
    public List<WorldMapNode> Children { get; set; } = new List<WorldMapNode>();
    public List<WorldMapNode> Parent { get; set; } = new List<WorldMapNode>();

    public Difficulty difficulty;
    public Vector2 index;
    public int level;

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < Parent.Count; i++)
        {
            if (Parent[i].index.Equals(player.CurrentIndex))
            {
                // ¾ÀÀüÈ¯(´øÀü¸ÊÀ¸·Î)
                //SceneManager.LoadScene(4);

                var pos = transform.position + new Vector3(0f, 1.5f, 0f);
                player.PlayerWorldMap(pos, index);
                return;
            }
        }
    }
}