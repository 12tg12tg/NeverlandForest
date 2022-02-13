using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomEventItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image selectedImg;

    private bool isSelect;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
                selectedImg.color = Color.white;
            else
                selectedImg.color = Color.clear;
        }
    }

    private DataAllItem dataItem;

    public DataAllItem DataItem { get => dataItem; }

    public void Init(DataAllItem data)
    {
        if (data == null)
        {
            dataItem = null;
            icon.sprite = null;
            count.text = string.Empty;
        }
        else
        {
            dataItem = data;
            AllItemTableElem elem = data.ItemTableElem;
            icon.sprite = elem.IconSprite;
            count.text = data.OwnCount.ToString();
        }
    }

    public void ItemButtonClick()
    {
        if (dataItem == null)
            return;

        RandomEventUIManager.Instance.info.Init(dataItem);
        RandomEventUIManager.Instance.info2page.Init(dataItem);
        
        IsSelect = !IsSelect;
        if(IsSelect)
            RandomEventUIManager.Instance.selectRewardItems.Add(dataItem);
        else
            RandomEventUIManager.Instance.selectRewardItems.Remove(dataItem);
    }
}
