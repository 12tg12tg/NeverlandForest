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

    public void OnPointerClick(PointerEventData eventData)
    {
        player.Move(index, transform.position);

        if (cloak != null)
            player.OnBush(index);

        ren.enabled = true;
    }
}
