using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public MeshRenderer ren;
    public Vector2 index;
    public bool isObstacle;
    public List<UnitBase> units = new List<UnitBase>();

    //Property
    public Vector3 WolrdPos { get => transform.position; }
    public bool HaveUnit { get => units.Count > 0; }
    public bool CanStand { get => units.Count < 2; }

    //Vars
    private Color tempOriginalColor;
    private bool isHighlightAttack;
    private bool isHighlightConsume;

    private void Start()
    {
        tempOriginalColor = ren.material.color;
    }

    public bool TryGetFowardTile(out Tiles tile, int distance)
    {
        var dest = index;
        dest.y -= distance;
        var destTile = TileMaker.Instance.GetTile(dest);
        
        if(destTile == null)
        {
            Debug.LogWarning($"Cant move foward beacuse there is no {dest} Index Tile!");
            tile = null;
            return false;
        }
        else
        {
            tile = destTile;
            return true;
        }
    }

    //Highlight
    public void HighlightSkillRange()
    {
        ren.material.color = Color.blue;
    }

    public void HighlightCanAttackSign()
    {
        isHighlightAttack = true;
        ren.material.color = Color.red;
    }

    public void HighlightCanConsumeSign()
    {
        isHighlightConsume = true;
        ren.material.color = Color.green;
    }

    public void SetOriginalState()
    {
        isHighlightAttack = false;
        isHighlightConsume = false;
        ren.material.color = tempOriginalColor;
    }

    public void SetMiddleState()
    {
        //하이라이트를 초기화하진 않으나, 마우스 드래그중에 표시되는 블루 프린트를 없앤다.
        if (isHighlightAttack)
            HighlightCanAttackSign();
        else if (isHighlightConsume)
            HighlightCanConsumeSign();
        else
            SetOriginalState();
    }

    //Drag Drop
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {index} Tile! ");
        TileMaker.Instance.LastDropPos = index;
    }

    //Click
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log($"{index} Clicked! ");
    }


}
