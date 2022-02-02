using UnityEngine;
using UnityEngine.UI;
public class GatheringInDungeonRewardObject : MonoBehaviour
{
    private DataAllItem item;
    private bool isSelect;
    [Header("버튼,아이콘")]
    public Button rewardButton;
    public Image rewardIcon;
    public Image selectedImg;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
                selectedImg.color = Color.blue;
            else
                selectedImg.color = Color.white;
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
        GatheringSystem.Instance.SelectedItem = item;
        IsSelect = true;
    }
    public void Init(DataAllItem data)
    {
        if (data == null)
        {
            item = null;
            rewardIcon.sprite = null;
        }
        else
        {
            item = new DataAllItem(data);
            AllItemTableElem elem = data.ItemTableElem;
            rewardIcon.sprite = elem.IconSprite;
        }
    }
}
