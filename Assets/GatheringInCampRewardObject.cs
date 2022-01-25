using UnityEngine;
using UnityEngine.UI;
public class GatheringInCampRewardObject : MonoBehaviour
{
    public Button rewardButton;
    public Image rewardIcon;
    private DataAllItem item;
    public Image selectedImg;
    private bool isSelect;

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
        var newItem = new DataAllItem();
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{randNum}";
        newItem.itemId = randNum;
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);
        newItem.LimitCount = 3;
        //buttonimage.sprite = newItem.ItemTableElem.IconSprite;
        if (randNum == 1)
        {
            //�����丷: 1 %
            item = newItem;
        }
        else if (randNum == 100)
        {
            //����: 1 %
            item = newItem;
        }
        else if (randNum >= 2 && randNum <= 4)
        {
            //��������3 %
            item = newItem;
        }
        else if (randNum >= 5 && randNum <= 9)
        {
            //����: 5 %
            item = newItem;
        }
        else if (randNum >= 10 && randNum <= 14)
        {
            // ����: 5 %
            item = newItem;
        }
        else
        {
            //��: 85 %
            Debug.Log("��");
            rewardIcon.sprite = Resources.Load<Sprite>($"Icons/xsymbol");
        }
        if (item != null)
        {
            CampManager.Instance.RewardList.Add(item);
        }
    }
    public void ItemButtonClick()
    {
        if (item !=null)
        {
            CampManager.Instance.SelectItem = item;
            IsSelect = true;
        }
    }
}
