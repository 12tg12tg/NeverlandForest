using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BottomItemButtonUI : MonoBehaviour
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
                selectedImg.color = Color.blue;
            else
                selectedImg.color = Color.white;
        }
    }

    private DataItem dataItem;

    public DataItem DataItem { get => dataItem; }

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
        BottomUIManager.Instance.info.Init(dataItem);
        BottomUIManager.Instance.selectItem = dataItem;
        BottomUIManager.Instance.popUpWindow.gameObject.SetActive(true);
        BottomUIManager.Instance.isPopUp = true;

        var uiVec = BottomUIManager.Instance.popUpWindow.position;
        var newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
        BottomUIManager.Instance.popUpWindow.position = newVector;
        // 선택초기화
        for (int i = 0; i < BottomUIManager.Instance.itemButtons.Count; i++)
        {
            BottomUIManager.Instance.itemButtons[i].IsSelect = false;
        }
        IsSelect = true;

    }

}

//    public void Init(DataConsumable data)
//{
//    dataItem = data;
//    ConsumableTableElem elem = data.ItemTableElem;

//    icon.sprite = elem.IconSprite;
//    count.text = data.OwnCount.ToString();
//    itemName.name = elem.name;
//}
//public void Init(DataMaterial data)
//{
//    dataItem = data;
//    AllItemTableElem elem = data.ItemTableElem;

//    icon.sprite = elem.IconSprite;
//    count.text = data.OwnCount.ToString();
//    itemName.name = elem.name;
//}