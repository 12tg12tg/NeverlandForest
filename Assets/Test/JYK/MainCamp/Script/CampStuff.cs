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
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            if (GameManager.Manager.tm.mainTutorial.tutorialCamp.IscraftFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartCook);
                GameManager.Manager.tm.mainTutorial.tutorialCamp.IscraftFinish = false;
            }
        }
        else
        {
            EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartCook);
        }
    }
    public void StartGathering()
    {

        if (GameManager.Manager.State == GameState.Tutorial)
        {
            if (GameManager.Manager.tm.mainTutorial.tutorialCamp.IssleepingFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartGathering);
                GameManager.Manager.tm.mainTutorial.tutorialCamp.IssleepingFinish = false;
            }
        }
        else
        {
             EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartGathering);
        }

    }
    public void StartSleep()
    {
        if (GameManager.Manager.State == GameState.Tutorial)
        {
            if (GameManager.Manager.tm.mainTutorial.tutorialCamp.IscookingFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartSleep);
                GameManager.Manager.tm.mainTutorial.tutorialCamp.IscookingFinish = false;
            }
        }
        else
        {
            EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartSleep);
        }


    }
    public void StartBlueMoon()
    {
        EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartBlueMoon);
    }
    public void StartMaking()
    {

        if (GameManager.Manager.State == GameState.Tutorial)
        {
            if (GameManager.Manager.tm.mainTutorial.tutorialCamp.IsTutorialFirst)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartMaking);

                GameManager.Manager.tm.mainTutorial.tutorialCamp.IsTutorialFirst = false;
            }
        }
        else
        {
            EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartMaking);

        }


    }
}
