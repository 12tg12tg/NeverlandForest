using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GatheringInDungeonRewardObject : MonoBehaviour
{
    private DataAllItem item;
    private bool isSelect;
    private bool isHaveItem;
    [Header("버튼,아이콘")]
    public Button rewardButton;
    public Image rewardIcon;
    public Image selectedImg;
    [SerializeField] private TextMeshProUGUI count;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                selectedImg.color = Color.white;
            }
            else
            {
                selectedImg.color = Color.clear;
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

        if(IsSelect)
            GatheringSystem.Instance.selecteditemList.Add(item);
        else
            GatheringSystem.Instance.selecteditemList.Remove(item);
    }
    public void Init(DataAllItem data)
    {
        if (data == null)
        {
            item = null;
            IsSelect = false;
            rewardIcon.sprite = null;
            rewardIcon.color = Color.clear;
            count.text = string.Empty;
        }
        else
        {
            item = new DataAllItem(data);
            AllItemTableElem elem = data.ItemTableElem;
            rewardIcon.color = Color.white;
            rewardIcon.sprite = elem.IconSprite;
            count.text = data.OwnCount.ToString();
        }
    }
}
