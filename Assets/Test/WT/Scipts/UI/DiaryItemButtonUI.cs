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
    [SerializeField] private Button ownButton;
    [SerializeField] private Image shadeCover;
    public Image Icon
    {
        get => icon;
    }
    private bool isSelect;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
                selectedImg.color = Color.white;
            else
                selectedImg.color = Color.clear;
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

        var diaryItemInstance = DiaryItem.Instance;
        Vector3 uiVec = Vector3.zero;
        Vector3 newVector = Vector3.zero;

        diaryItemInstance.info.Init(dataItem);
        diaryItemInstance.selectedItem = dataItem;
        diaryItemInstance.itemButtons.ForEach(n => n.IsSelect = false);
        diaryItemInstance.selectedItemSlotNum = slotNum;
        diaryItemInstance.itemPopup.gameObject.SetActive(true);
        diaryItemInstance.isPopUp = true;
        diaryItemInstance.selectItemRect = gameObject.GetComponent<RectTransform>();
        uiVec = diaryItemInstance.itemPopup.transform.position;
        newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
        diaryItemInstance.itemPopup.transform.position = newVector;
        diaryItemInstance.itemButtons.ForEach(n => n.IsSelect = false);
        IsSelect = true;
    }
}
