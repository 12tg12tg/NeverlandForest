using UnityEngine;
using UnityEngine.UI;
public class GatheringInDungeonRewardObject : MonoBehaviour
{
    private DataAllItem item;
    private bool isSelect;
    private bool isHaveItem;
    [Header("버튼,아이콘")]
    public Button rewardButton;
    public Image rewardIcon;
    public Image selectedImg;
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
            }
            else
            {
                selectedImg.color = Color.clear;
                selectedImg.sprite = null;
            }
        }
    }
    public bool IsHaveItem
    {
        get => isHaveItem;
        set
        {
            isHaveItem = value;
        }
    }

    public DataAllItem Item
    {
        get => item;
        set
        {
            item = value;
        }
    }
    public void ItemButtonClick()
    {
        if (item == null)
            return;
        IsSelect = !IsSelect;
        GatheringSystem.Instance.selecteditemList.Add(item);
    }
    public void Init(DataAllItem data)
    {
        if (data == null)
        {
            item = null;
            rewardIcon.sprite = null;
            rewardIcon.color = Color.clear;
        }
        else
        {
            item = new DataAllItem(data);
            AllItemTableElem elem = data.ItemTableElem;
            rewardIcon.color = Color.white;
            rewardIcon.sprite = elem.IconSprite;
        }
    }
}
