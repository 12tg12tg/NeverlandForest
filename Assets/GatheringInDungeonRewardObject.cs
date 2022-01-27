using UnityEngine;
using UnityEngine.UI;
public class GatheringInDungeonRewardObject : MonoBehaviour
{
    public Button rewardButton;
    public Image rewardIcon;
    private DataAllItem item;
    public Image selectedImg;
    private bool isSelect;
    public  DataAllItem staticData;
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
    private void Update()
    {
        //Debug.Log(item);
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
            staticData = new DataAllItem(data);
            item = staticData;
            AllItemTableElem elem = data.ItemTableElem;
            rewardIcon.sprite = elem.IconSprite;
        }
    }
    public void Test(DataAllItem data)
    {
        item = data;
        Debug.Log($"{item} {staticData}");
    }
}
