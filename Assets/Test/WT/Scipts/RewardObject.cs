using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardObject : MonoBehaviour
{
    private DataAllItem item;
    public DataAllItem Item { get => item; set => item = value; }

    private bool isSelect = false;

    public Image selectedImg;
    public Image itemIcon;

    public Sprite selectedImage;

    public TextMeshProUGUI count;

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
                DungeonRewardDiaryManager.Instance.selectedItemList.Add(this);
            }
            else
            {
                selectedImg.color = Color.clear;
                selectedImg.sprite = null;
                DungeonRewardDiaryManager.Instance.selectedItemList.Remove(this);
            }

        }
    }

    public void Init(DataAllItem item)
    {
        if (item != null)
        {
            Item = item;
            itemIcon.sprite = item.ItemTableElem.IconSprite;
            itemIcon.color = Color.white;
            count.enabled = true;
            count.text = item.OwnCount.ToString();
        }
        else
        {
            InitItemSprite();
            count.enabled = false; 
        }
    }


    public void ItemButtonClick() // 버튼 클릭 함수
    {
        if (item == null)
            return;
        DungeonRewardDiaryManager.Instance.info.Init(item);
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

        if(count != null)
            count.enabled = false;
    }
}
