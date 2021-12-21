using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICookInventoryList : MonoBehaviour
{
    public CookObject itemPrefab;
    public ScrollRect scrollRect;

    public const int MaxItemCount = 20;
    public List<CookObject> itemGoList = new List<CookObject>();
    public List<DataMaterial> alllist = new List<DataMaterial>();
    private int selectedSlot = -1;

    public Image Fire;
    public Image Condiment;
    public Image Material;
    public Image Result;

    public CookObject FireObject;
    public CookObject CondimentObject;
    public CookObject MaterialObject;
    public CookObject ResultObject;

    private CookInventory ci;
    private CookMaterialTypes curType;

    private void Awake()
    {
        for (var i = 0; i < MaxItemCount; ++i)
        {
            var item = Instantiate(itemPrefab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.AddComponent<Button>();
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnChangedSelection(item.Slot));

        }
      
    }
    public void Start()
    {
        FireObject = itemGoList[0];
        Fire.sprite = itemGoList[0].icon;
    }
    public void Init(List<DataMaterial> itemList, CookMaterialTypes type)
    {
        alllist = itemList;
        SetAllItems(alllist);
        curType = type;
    }
    public void SetAllItems(List<DataMaterial> itemList)
     {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            itemGoList[i].Init(itemList[i]);
            itemGoList[i].gameObject.SetActive(true);
        }
        if (itemList.Count > 0)
        {
            selectedSlot = 0;
            EventSystem.current.SetSelectedGameObject(itemGoList[selectedSlot].gameObject);
        }
    }
    public void OnChangedSelection(int slot)
    {
        var type = itemGoList[slot].DataItem.ItemTableElem.type;

        switch (type)
        {
            case "NONE":
                Fire.sprite = itemGoList[slot].icon;
                FireObject = itemGoList[slot];
                break;
            case "FIRE":
                Fire.sprite = itemGoList[slot].icon;
                FireObject = itemGoList[slot];
                break;
            case "CONDIMENT":
                Condiment.sprite = itemGoList[slot].icon;
                CondimentObject = itemGoList[slot];
                break;
            case "FOODINGREDIENT":
                Material.sprite = itemGoList[slot].icon;
                MaterialObject = itemGoList[slot];
                break;
            case "FOOD":
                break;
            default:
                break;
        }
        selectedSlot = slot;
    }

    public void RemoveFireIcon()
    {
        Fire.sprite = null;
        FireObject = null;
    }
    public void RemoveCondimetnIcon()
    {
        Condiment.sprite = null;
        CondimentObject = null;
    }
    public void RemoveMaterialIcon()
    {
        Material.sprite = null;
        MaterialObject = null;
    }
    public void GetResultIcon()
    {
        if (FireObject!=null && CondimentObject != null && MaterialObject != null
            && Fire.sprite != null && Condiment.sprite != null && Material.sprite != null)
        {   
            
            var list = Vars.UserData.HaveMaterialList;
            if (ResultObject !=null)
            {
                list.Add(ResultObject.DataItem);
                for (int i = 0; i < list.Count; i++)
                {
                    Debug.Log(list[i].ItemTableElem.name);
                }
            }
            Result.sprite = null;
            ResultObject = null;
            Fire.sprite = null;
            FireObject = null;
            Condiment.sprite = null;
            CondimentObject = null;
            Material.sprite = null;
            MaterialObject = null;
        }
    }
}
