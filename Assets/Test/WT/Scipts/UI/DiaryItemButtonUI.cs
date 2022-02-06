using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryItemButtonUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image selectedImg;
    [SerializeField] private Image type;
    [SerializeField] private Button ownButton;
    [SerializeField] private Image shadeCover;
    private bool isSelect;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
                selectedImg.color = Color.red;
            else
                selectedImg.color = Color.green;
        }
    }
    private DataAllItem dataItem;

    public DataAllItem DataItem { get => dataItem; }
    private int slotNum;
   
    public void Init(DataAllItem data, int slot = -1)
    {
        slotNum = slot;

        count.color = Color.black;
        if (data == null)
        {
            dataItem = null;
            icon.sprite = null;
            count.text = string.Empty;
            IsSelect = false;
            return;
        }
        dataItem = data;
        AllItemTableElem elem = data.ItemTableElem;
        icon.sprite = elem.IconSprite;
        count.text = data.OwnCount.ToString();
    }
    public void ItemButtonClick()
    {
        if (dataItem == null)
            return;
        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;
        DiaryItem.Instance.info.Init(dataItem);
        DiaryItem.Instance.itemButtons.ForEach(n => n.IsSelect = false);
    }
}
