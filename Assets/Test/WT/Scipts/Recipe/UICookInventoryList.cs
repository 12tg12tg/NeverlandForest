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
    
    public Image fire;
    public Image condiment;
    public Image material;
    public Image result;
    public Sprite xSprite;

    public CookObject fireObject;
    public CookObject condimentObject;
    public CookObject materialObject;
    public CookObject resultObject;

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
        fire.sprite = xSprite;
        fireObject = itemGoList[0];

        condiment.sprite = xSprite;
        condimentObject = null;

        material.sprite = xSprite;
        materialObject = null;
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
                fire.sprite = itemGoList[slot].icon;
                fireObject = itemGoList[slot];
             
                break;
            case "FIRE":
                fire.sprite = itemGoList[slot].icon;
                fireObject = itemGoList[slot];
                break;
            case "CONDIMENT":
                condiment.sprite = itemGoList[slot].icon;
                condimentObject = itemGoList[slot];
                break;
            case "FOODINGREDIENT":
                material.sprite = itemGoList[slot].icon;
                materialObject = itemGoList[slot];
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
        if (resultObject == null)
        {
            fire.sprite = null;
            fireObject = null;
        }

    }
    public void RemoveCondimetnIcon()
    {
        if (resultObject == null)
        {
            condiment.sprite = null;
            condimentObject = null;
        }

    }
    public void RemoveMaterialIcon()
    {
        if (resultObject == null)
        {
            material.sprite = null;
            materialObject = null;
        }

    }
    public void GetResultIcon()
    {
       /* if (fireObject != null && condimentObject != null && materialObject != null
            && fire.sprite != null && condiment.sprite != null && material.sprite != null)
        {
            var list = Vars.UserData.HaveMaterialList;
            if (resultObject != null)
            {
                list.Add(resultObject.DataItem);
                for (int i = 0; i < list.Count; i++)
                {
                    Debug.Log(list[i].ItemTableElem.name);
                }
            }
            result.sprite = null;
            resultObject = null;

            fire.sprite = xSprite;
            fireObject = itemGoList[0];

            condiment.sprite = xSprite;
            condimentObject = null;

            material.sprite = xSprite;
            materialObject = null;
        }*/
    }
}
