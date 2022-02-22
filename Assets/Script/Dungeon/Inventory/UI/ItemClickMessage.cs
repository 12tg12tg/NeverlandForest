using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//public class ItemClickMessage : GenericWindow
//{
//    [Header("ETC")]
//    [Space(3)]
//    public Button[] buttons;
//    public Image itemImg;
//    public RectTransform buttonPanel;
//    private DataItem itemInfo;

//    private UnitBase playerUnit;

//    new void Awake()
//    {
//        buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "���";
//        buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "������";
//        buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = "ĭ ������";
//        buttons[3].GetComponentInChildren<TextMeshProUGUI>().text = "���";

//        buttons[0].onClick.AddListener(() => OnClickItemUse());
//        buttons[1].onClick.AddListener(() => OnClickItemRemove());
//        buttons[2].onClick.AddListener(() => OnClickItemAllRemove());
//        buttons[3].onClick.AddListener(() => Close());
//        for (int i = 0; i < buttons.Length; i++)
//        {
//            buttons[i].gameObject.SetActive(false);
//        }
//    }
//    public override void Open()
//    {
//        base.Open();
//    }
//    public override void Close()
//    {
//        buttons[0].gameObject.SetActive(false);
//        buttons[1].gameObject.SetActive(false);
//        buttons[2].gameObject.SetActive(false);
//        buttons[3].gameObject.SetActive(false);
//        base.Close();
//    }

//    public void SetPlayerUnit(UnitBase unitInfo)
//    {
//        playerUnit = unitInfo;
//    }

//    public void Init(DataItem item)
//    {
//        itemInfo = item;
//        switch (itemInfo.dataType)
//        {
//            case DataType.Consume:
//                break;
//            case DataType.AllItem:
//                var allItem = new DataAllItem(itemInfo);
//                itemImg.sprite = allItem.ItemTableElem.IconSprite;
//                buttons[0].gameObject.SetActive(false);
//                buttons[1].gameObject.SetActive(false);
//                buttons[2].gameObject.SetActive(false);
//                buttons[3].gameObject.SetActive(false);

//                if (allItem.ItemTableElem.type == "FOOD")
//                {
//                    buttons[0].gameObject.SetActive(true);
//                    buttons[1].gameObject.SetActive(true);
//                    buttons[2].gameObject.SetActive(true);
//                    buttons[3].gameObject.SetActive(true);
//                }
//                else
//                {
//                    buttons[1].gameObject.SetActive(true);
//                    buttons[2].gameObject.SetActive(true);
//                    buttons[3].gameObject.SetActive(true);
//                }
//                break;
//        }
//    }

//    private void OnClickItemUse()
//    {
//        // �ϴ� 1����, �ӽñ���

//        switch (itemInfo.dataType)
//        {
//            case DataType.Consume:
//                break;
//            case DataType.AllItem:
//                var useItem = new DataAllItem(itemInfo);
//                useItem.OwnCount = 1;
//                if (useItem.ItemTableElem.type == "FOOD")
//                {
//                    Vars.UserData.uData.CurStamina += 10f;
//                    if(playerUnit != null)
//                        playerUnit.Hp += 10;
//                    if(Vars.UserData.RemoveItemData(useItem))
//                        Close();
//                }
//                break;
//        }
//        // TODO : ���߿� ��� �����ϱ�
//        Debug.Log("������ ���!");
//    }

//    private void OnClickItemRemove()
//    {
//        switch (itemInfo.dataType)
//        {
//            case DataType.Consume:
//                break;
//            case DataType.AllItem:
//                var allItem = new DataAllItem(itemInfo);
//                allItem.OwnCount = 1;
//                if(Vars.UserData.RemoveItemData(allItem))
//                {
//                    Close();
//                }
//                break;
//        }
//        Debug.Log("������ �ϳ� ������!");
//        invenCtrl.Init();

//    }

//    private void OnClickItemAllRemove()
//    {
//        switch (itemInfo.dataType)
//        {
//            case DataType.Consume:
//                break;
//            case DataType.AllItem:
//                var allItem = new DataAllItem(itemInfo);
//                allItem.OwnCount = itemInfo.OwnCount;
//                Vars.UserData.RemoveItemData(allItem);
//                break;
//        }
//        Debug.Log("������ ĭ ������ ������!");
//        invenCtrl.Init();
//        Close();
//    }
//}
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