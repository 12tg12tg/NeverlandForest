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
                selectedImg.color = Color.red;
            else
                selectedImg.color = Color.green;
        }
    }

    private DataAllItem dataItem;

    public DataAllItem DataItem { get => dataItem; }

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
        if (dataItem == null)
            return;
        // TODO: 임시, 가라로 해놓은거
        if (RandomEventManager.Instance != null && GameManager.Manager.State == GameState.Dungeon)
        {
            RandomEventUIManager.Instance.info.Init(dataItem);
            RandomEventUIManager.Instance.info2page.Init(dataItem);
            RandomEventUIManager.Instance.selectInvenItem = dataItem;
            //RandomEventUIManager.Instance.popUpWindow.gameObject.SetActive(true);
            RandomEventUIManager.Instance.itemBox = gameObject.GetComponent<RectTransform>();
            RandomEventUIManager.Instance.isPopUp = true;

            var uiVec = RandomEventUIManager.Instance.popUpWindow.position;
            var newVector = new Vector3(transform.position.x, uiVec.y, uiVec.z);
            RandomEventUIManager.Instance.popUpWindow.position = newVector;
            // 선택초기화
            for (int i = 0; i < RandomEventUIManager.Instance.itemButtons.Count; i++)
            {
                RandomEventUIManager.Instance.itemButtons[i].IsSelect = false;
                RandomEventUIManager.Instance.itemButtons2page[i].IsSelect = false;
            }
            IsSelect = true;
        }

        else
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
            if (DiaryInventory.Instance != null)
            {
                DiaryInventory.Instance.info.Init(dataItem);
                Debug.Log("다이어리", DiaryInventory.Instance.info);
                // 선택초기화
                for (int i = 0; i < DiaryInventory.Instance.itemButtons.Count; i++)
                {
                    DiaryInventory.Instance.itemButtons[i].IsSelect = false;
                }
            }
        }
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