using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterClick : MonoBehaviour, IPointerClickHandler
{
    public MonsterUnit unit;
    private BottomUIManager bottomUi;

    public void Start()
    {
        bottomUi = BottomUIManager.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bottomUi.info.Init(unit);
    }
}
