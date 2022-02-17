using UnityEngine;
using UnityEngine.UI;
public class GatheringInCampRewardObject : MonoBehaviour
{
    private DataAllItem item;
    private bool isSelect;

    private string stringid = string.Empty;
    private bool isBlank = false;
    [Header("¹öÆ° ¼ÂÆÃ")]
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
    public DataAllItem Item
    {
        get => item;
        set { item = value; }
    }
    public bool IsBlank
    {
        get => isBlank;
        set { isBlank = value; }
    }
 
    public void SetRewardItemIcon()
    {
        //³ª¹«Åä¸·: 1 %
        //¾¾¾Ñ: 1 %
        //³ª¹µ°¡Áö3 %
        //¾àÃÊ: 5 %
        // ¹ö¼¸: 5 %
        //²Î: 85 %
        var randNum = Random.Range(1, 101);
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();

        if (GameManager.Manager.State ==GameState.Tutorial)
        {
            randNum = 95;
            isBlank = true;
        }

        if (randNum == 1)
        {
            //³ª¹«Åä¸·: 1 %
            stringid = $"ITEM_1";
            isBlank = false;

        }
        else if (randNum == 2)
        {
            //¾¾¾Ñ: 1 %
            stringid = $"ITEM_3";
            isBlank = false;

        }
        else if (randNum >= 3 && randNum <= 5)
        {
            //³ª¹µ°¡Áö3 %
            stringid = $"ITEM_2";
            isBlank = false;

        }
        else if (randNum >= 6 && randNum <= 10)
        {
            //¾àÃÊ: 5 %
            stringid = $"ITEM_4";
            isBlank = false;

        }
        else if (randNum >= 11 && randNum <= 15)
        {
            // ¹ö¼¸: 5 %
            stringid = $"ITEM_6";
            isBlank = false;

        }
        else
        {
            //²Î: 85 %
            selectedImg.sprite = null;
            isBlank = true;
        }

        if (stringid != string.Empty)
        {
            var newItem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringid))
            {
                OwnCount = Random.Range(1, 3)
            };
            rewardIcon.sprite = newItem.ItemTableElem.IconSprite;
            rewardIcon.color = Color.white;
            selectedImg.color = Color.clear;
            item = newItem;
        }
        else
        {
            rewardIcon.sprite =null;
            rewardIcon.color = Color.clear;
        }
    }
    public void ItemButtonClick()
    {
        if (item == null)
            return;
            IsSelect = !IsSelect;
        if (IsSelect)
        {
            selectedImg.sprite = selectedImage;
            selectedImg.color = Color.white;
        }
        else
        {
            selectedImg.sprite = null;
            selectedImg.color = Color.clear;

        }
    }
}
