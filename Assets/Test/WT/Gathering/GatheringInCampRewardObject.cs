using UnityEngine;
using UnityEngine.UI;
public class GatheringInCampRewardObject : MonoBehaviour
{
    public Button rewardButton;
    public Image rewardIcon;
    private DataAllItem item;
    public Image selectedImg;
    private bool isSelect;
    private string stringid = string.Empty;
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
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }

    public bool isBlank =false;

    public void Awake()
    {
        SetRewardItemIcon();
    }

    public void SetRewardItemIcon()
    {
        //�����丷: 1 %
        //����: 1 %
        //��������3 %
        //����: 5 %
        // ����: 5 %
        //��: 85 %
        var randNum = Random.Range(1, 101);

        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();


        //buttonimage.sprite = newItem.ItemTableElem.IconSprite;
        if (randNum == 1)
        {
            //�����丷: 1 %
            stringid = $"ITEM_1";
        }
        else if (randNum == 2)
        {
            //����: 1 %
            stringid = $"ITEM_3";
        }
        else if (randNum >= 3 && randNum <= 5)
        {
            //��������3 %
            stringid = $"ITEM_2";
        }
        else if (randNum >= 6 && randNum <= 10)
        {
            //����: 5 %
            stringid = $"ITEM_4";
        }
        else if (randNum >= 11 && randNum <= 15)
        {
            // ����: 5 %
            stringid = $"ITEM_6";
        }
        else
        {
            //��: 85 %
            Debug.Log("��");
            rewardIcon.sprite = Resources.Load<Sprite>($"Icons/xsymbol");
            isBlank = true;
        }

        if (stringid != string.Empty)
        {
            var newItem = new DataAllItem(allitemTable.GetData<AllItemTableElem>(stringid))
            {
                OwnCount = Random.Range(1, 3)
            };
            rewardIcon.sprite = newItem.ItemTableElem.IconSprite;
            item = newItem;
            CampManager.Instance.RewardList.Add(item);
        }

    }
    public void ItemButtonClick()
    {
        if (item != null)
        {
            CampManager.Instance.SelectItem = item;
            IsSelect = true;
        }
    }
}
