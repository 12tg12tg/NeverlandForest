using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryItem : MonoBehaviour
{
    public int Slot { get; set; }
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI count;

    private DataItem dataItem;

    public DataItem DataItem { get => dataItem; }

    public void Init(DataAllItem data)
    {
        dataItem = data;
        AllItemTableElem elem = data.ItemTableElem;

        icon.sprite = elem.IconSprite;
        itemName.text = elem.name;
        count.text = data.OwnCount.ToString();
    }

    //public void Init(DataCunsumable data)
    //{
    //    dataItem = data;
    //    ItemTableElem elem = data.ItemTableElem;

    //    icon.sprite = elem.IconSprite;
    //    text.text = elem.name;

    //    number.gameObject.SetActive(true);
    //    number.text = data.count.ToString();
    //}

    //public void Init(DataWeapon data)
    //{
    //    if (data == null)
    //    {
    //        icon.sprite = null;
    //        text.text = string.Empty;
    //        return;
    //    }

    //    dataItem = data;
    //    WeaponTableElem elem = data.ItemTableElem;

    //    icon.sprite = elem.IconSprite;
    //    text.text = elem.name;
    //    number.gameObject.SetActive(false);
    //}
}
