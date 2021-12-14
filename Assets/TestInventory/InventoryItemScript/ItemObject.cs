using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    public int Slot { get; set; }
    public Image icon;
    public TextMeshProUGUI nameText;

    private DataItem dataItem;

    public DataItem DataItem { get => dataItem; }

    public void Init(DataCunsumable data)
    {
        if (data == null)
        {
            icon.sprite = null;
            nameText.text = string.Empty;
            return;
        }

        dataItem = data;
        ItemTableElem elem = data.ItemTableElem;

        icon.sprite = elem.IconSprite;
        nameText.text = elem.name;
    }

    public void Init(DataWeapon data)
    {
        if (data == null)
        {
            icon.sprite = null;
            nameText.text = string.Empty;
            return;
        }

        dataItem = data;
        WeaponTableElem elem = data.ItemTableElem;
        icon.sprite = elem.IconSprite;
        nameText.text = elem.name;
    }
}
