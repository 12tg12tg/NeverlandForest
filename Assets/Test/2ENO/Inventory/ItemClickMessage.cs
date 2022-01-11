using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemClickMessage : GenericWindow
{
    [Header("ETC")]
    [Space(3)]
    public Button[] buttons;
    public Image itemImg;
    public RectTransform buttonPanel;
    public InventoryController invenCtrl;
    private DataItem itemInfo;

    new void Awake()
    {
        buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "���";
        buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "������";
        buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = "���";

        buttons[0].onClick.AddListener(() => OnClickItemUse());
        buttons[1].onClick.AddListener(() => OnClickItemRemove());
        buttons[2].onClick.AddListener(() => Close());
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        buttons[0].gameObject.SetActive(false);
        buttons[1].gameObject.SetActive(false);
        buttons[2].gameObject.SetActive(false);
        base.Close();
    }

    public void Init(DataItem item)
    {
        itemInfo = item;
        switch (itemInfo.dataType)
        {
            case DataType.Default:
                break;
            case DataType.Consume:
                break;
            case DataType.AllItem:
                var allItem = new DataAllItem(itemInfo);
                itemImg.sprite = allItem.ItemTableElem.IconSprite;
                buttons[0].gameObject.SetActive(false);
                buttons[1].gameObject.SetActive(false);
                buttons[2].gameObject.SetActive(false);

                if (allItem.ItemTableElem.type == "FOOD")
                {
                    buttons[0].gameObject.SetActive(true);
                    buttons[1].gameObject.SetActive(true);
                    buttons[2].gameObject.SetActive(true);
                }
                else
                {
                    buttons[1].gameObject.SetActive(true);
                    buttons[2].gameObject.SetActive(true);
                }
                break;
        }
    }

    private void OnClickItemUse()
    {
        // �ϴ� 1����
        itemInfo.OwnCount = 1;
        // TODO : ���߿� ��� �����ϱ�
        Debug.Log("������ ���!");
    }

    private void OnClickItemRemove()
    {
        itemInfo.OwnCount = 1;
        Debug.Log("������ ������!");
        Vars.UserData.RemoveItemData(itemInfo);
    }

}
//var btn = Instantiate(buttonPrefab, buttonPanel);
//if (i == 0)
//{
//    btn.GetComponentInChildren<TextMeshProUGUI>().text = "���";
//    btn.onClick.AddListener(() => i = 1);
//}
//else if (i == 1)
//{
//    btn.GetComponentInChildren<TextMeshProUGUI>().text = "������";
//}
//else
//{
//    btn.GetComponentInChildren<TextMeshProUGUI>().text = "���";
//}

//selectButton.Add(btn);