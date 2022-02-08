using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
    private DataAllItem item;
    public DataAllItem Item { get => item; set => item = value; }

    private bool isSelect = false;

    public Image selectedImg;
    public Image itemIcon;

    public Sprite selectedImage;

    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                selectedImg.color = Color.white;
                selectedImg.sprite = selectedImage;
                DungeonRewardDiaryManager.Instacne.selectedItemList.Add(this);
            }
            else
            {
                selectedImg.color = Color.clear;
                selectedImg.sprite = null;
                DungeonRewardDiaryManager.Instacne.selectedItemList.Remove(this);
            }

        }
    }

    public void ItemButtonClick()
    {
        if (item == null)
            return;
        IsSelect = !IsSelect;
    }

    public void SetItemSprite(Sprite sprite)
    {
        itemIcon.sprite = sprite;
        itemIcon.color = Color.white;
    }
    public void InitItemSprite()
    {
        item = null;

        isSelect = true;

        itemIcon.sprite = null;
        itemIcon.color = Color.clear;

        selectedImg.sprite = null;
        selectedImg.color = Color.clear;
    }
}
