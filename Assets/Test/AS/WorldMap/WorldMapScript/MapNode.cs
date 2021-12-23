using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


[Serializable]
public class MapNode : MonoBehaviour, IPointerClickHandler
{
    public TestPlayer player;
    
    public List<MapNode> children = new List<MapNode>();
    public List<MapNode> parent = new List<MapNode>();
    public List<MapNode> Children { get => children; set => children = value; }
    public List<MapNode> Parent { get => parent; set => parent = value; }

    public Vector2 index;
    public int level;

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < parent.Count; i++)
        {
            if (parent[i].index.Equals(player.CurrentIndex))
            {
                // ¾ÀÀüÈ¯(´øÀü¸ÊÀ¸·Î)
                //SceneManager.LoadScene(4);

                var pos = gameObject.transform.position + new Vector3(0f, 1.5f, 0f);
                player.PlayerWorldMap(pos, index);
                return;
            }
            else
                Debug.Log("ÀÌµ¿ xxx");
        }
    }
}