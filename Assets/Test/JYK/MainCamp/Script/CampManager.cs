using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampManager : MonoBehaviour
{
    public enum CampEvent
    {
        StartInventory,
        StartCook,
        StartEquipment
    }

    public void OnEnable()
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartInventory, OpenInventoryWindow);
        EventBus<CampEvent>.Subscribe(CampEvent.StartEquipment, OpenEquipmentWindow);
    }
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartInventory, OpenInventoryWindow);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartEquipment, OpenEquipmentWindow);
        EventBus<CampEvent>.ResetEventBus();
    }

    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Cooking Scene");
        SceneManager.LoadScene("wt_recipe");
    }

    public void OpenInventoryWindow(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Inventory Window");
    }

    public void OpenEquipmentWindow(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Equipment Window");
    }
}
