//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Linq;
//public enum InventoryTypes
//{
//    Weapon,
//    Armor,
//    Consumable
//}

//public class UIItemInventory : MonoBehaviour
//{
//    private InventoryTypes currentInventory;
//    public InventoryTypes CurrentInventory
//    {
//        get { return currentInventory; }
//    }

//    public UIInventoryItemList itemList;
 //  public UIInventoryItemInfo itemInfo;

//    public ItemPopUpList itemPopUp;

//    private void Start()
//    {
//        SetInventoryType(InventoryTypes.Weapon);
//    }

//    public void SetInventoryType(InventoryTypes newTypes)
//    {
//        List<DataItem> list = null;
//        currentInventory = newTypes;
//        switch (currentInventory)
//        {
//            case InventoryTypes.Weapon:
//                list = Vars.UserData.weaponItemList.Cast<DataItem>().ToList();
//                break;
//            case InventoryTypes.Consumable:
//                list = Vars.UserData.consumableItemList.Cast<DataItem>().ToList();
//                break;
//        }

//        itemList.Init(currentInventory, list);
//    }

//    public void OnClickWeapon(bool check)
//    {
//        if (check)
//        {
//            SetInventoryType(InventoryTypes.Weapon);
//        }
//    }
//    public void OnClickArmor(bool check)
//    {
//        if (check)
//        {
//            SetInventoryType(InventoryTypes.Armor);
//        }
//    }
//    public void OnClickConsume(bool check)
//    {
//        if (check)
//        {
//            SetInventoryType(InventoryTypes.Consumable);
//        }
//    }

//    // 테스트로 버튼클릭으로 구현하지만 실제 사용시엔 이벤트 버스 패턴에 등록되어 사용될수도?
//    public void OnAddItem()
//    {
//        itemPopUp.gameObject.SetActive(true);
//        itemPopUp.isDelete = false;
//        var list = new List<DataItem>();
//        list.AddRange(Vars.WeaponItemList);
//        list.AddRange(Vars.ConsumableItemList);
//        itemPopUp.init(list);

//        //var list2 = new List<DataItem>();
//        //list2.AddRange(Vars.UserData.weaponItemList);
//        //list2.AddRange(Vars.UserData.consumableItemList);
//        //itemPopUp.init2(list2);
//    }

//    public void OnDeleteItem()
//    {
//        itemPopUp.gameObject.SetActive(true);
//        itemPopUp.isDelete = true;
//        //var list = new List<DataItem>();
//        //list.AddRange(Vars.WeaponItemList);
//        //list.AddRange(Vars.ConsumableItemList);
//        //itemPopUp.init(list);


//        var list2 = new List<DataItem>();
//        list2.AddRange(Vars.UserData.weaponItemList);
//        list2.AddRange(Vars.UserData.consumableItemList);
//        itemPopUp.init2(list2);
//    }

//    public void OnItemUse()
//    {

//    }

//    //public void UseItem(DataCharacter character)
//    //{
//    //    itemList.OnUseItemMode(character);
//    //}
//}
