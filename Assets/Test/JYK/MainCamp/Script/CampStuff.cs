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
            if (CampManager.Instance.camptutorial.IscraftFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartCook);
                CampManager.Instance.camptutorial.IscraftFinish = false;
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
            if (CampManager.Instance.camptutorial.IssleepingFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartGathering);
                CampManager.Instance.camptutorial.IssleepingFinish = false;
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
            if (CampManager.Instance.camptutorial.IscookingFinish)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartSleep);
                CampManager.Instance.camptutorial.IscookingFinish = false;
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
        BattleManager.initState = BattleInitState.BluemoonSet;
        GameManager.Instance.LoadScene(GameScene.Battle);
    }
    public void StartMaking()
    {

        if (GameManager.Manager.State == GameState.Tutorial)
        {
            if (CampManager.Instance.camptutorial.IsTutorialFirst)
            {
                EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartMaking);

                CampManager.Instance.camptutorial.IsTutorialFirst = false;
            }
        }
        else
        {
            EventBus<CampManager.CampEvent>.Publish(CampManager.CampEvent.StartMaking);

        }


    }
}
