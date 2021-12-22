using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    public enum CampEvent
    {
        StartInventory,
        StartCook,
        StartEquipment
    }

    public void Init() //GM의 Start에서 호출.
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartInventory, OpenInventoryWindow);
        EventBus<CampEvent>.Subscribe(CampEvent.StartEquipment, OpenEquipmentWindow);
    }

    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Cooking Scene");
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

    private new void OnDestroy()
    {
        base.OnDestroy();
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartInventory, OpenInventoryWindow);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartEquipment, OpenEquipmentWindow);
        EventBus<CampEvent>.ResetEventBus();
    }
}
