using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CampStuff : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent clickEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent.Invoke();
        Debug.Log("Click");
    }

    public void StartCook()
    {
        EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartCook);
    }
    public void StartGathering()
    {
        EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartGathering);
    }
    public void StartSleep()
    {
        EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartSleep);
    }
}
