using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CampStuff : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent clickEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent.Invoke();
    }

    public void StartCook()
    {
        EventBus<MainManager.CampEvent>.Publish(MainManager.CampEvent.StartCook);
    }

    public void StartEquipment()
    {
        EventBus<MainManager.CampEvent>.Publish(MainManager.CampEvent.StartEquipment);
    }

    public void StartInventory()
    {
        EventBus<MainManager.CampEvent>.Publish(MainManager.CampEvent.StartInventory);
    }
}
