using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryItemInfoUI : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI info_name;
    public TextMeshProUGUI info_description;
    public void Init()
    {
        img.sprite = null;
        info_name.text = "���� ����";
        info_description.text = "���� ����";
    }
    public void Init(DataAllItem item)
    {
        if (item == null)
            return;

        var allItem = item;
        img.sprite = allItem.ItemTableElem.IconSprite;
        info_name.text = allItem.ItemTableElem.name;
        info_description.text = allItem.ItemTableElem.desc;
    }
}
