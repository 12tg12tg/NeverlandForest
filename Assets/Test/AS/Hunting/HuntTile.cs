using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HuntTile : MonoBehaviour, IPointerClickHandler
{
    public Bush cloak;

    public HuntPlayer player;
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    public void OnPointerClick(PointerEventData eventData)
    {
<<<<<<< HEAD:Assets/Test/JYK/TileMaker/Tiles.cs
        Debug.Log($"{index} Clicked!");
        //ren.enabled = true;
=======
        player.Move(index, transform.position);

        if (cloak != null)
            player.OnBush(index);

        ren.enabled = true;
>>>>>>> AS_V3:Assets/Test/AS/Hunting/HuntTile.cs
    }

  

}
