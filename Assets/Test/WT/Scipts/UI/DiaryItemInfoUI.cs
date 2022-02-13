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
    [SerializeField] private List<DiaryItemButtonUI> diaryItemList = new List<DiaryItemButtonUI>();

    public void Init()
    {
        img.sprite = diaryItemList[0].Icon.sprite;
        info_name.text = diaryItemList[0].DataItem.ItemTableElem.name;
        info_description.text = diaryItemList[0].DataItem.ItemTableElem.desc;
    }
    public void Init(DataAllItem item)
    {
        if (item == null)
            return;

        var allItem = item;
        img.sprite = allItem.ItemTableElem.IconSprite;
        img.color = Color.white;
        info_name.text = allItem.ItemTableElem.name;
        info_description.text = allItem.ItemTableElem.desc;
    }
}
