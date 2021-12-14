using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public enum InventoryTypes
{
    Weapon,
    Armor,
    Consumable
}

public class UIItemInventory : MonoBehaviour
{
    private InventoryTypes currentInventory;
    public InventoryTypes CurrentInventory
    {
        get { return currentInventory; }
    }

    public UIInventoryItemList itemList;
    public UIInventoryItemInfo itemInfo;

    public ItemPopUpList itemPopUp;

    private void Start()
    {
        SetInventoryType(InventoryTypes.Weapon);
    }

    public void SetInventoryType(InventoryTypes newTypes)
    {
        List<DataItem> list = null;
        currentInventory = newTypes;
        switch (currentInventory)
        {
            case InventoryTypes.Weapon:
                list = Vars.UserData.weaponItemList.Cast<DataItem>().ToList();
                break;
            case InventoryTypes.Consumable:
                list = Vars.UserData.consumableItemList.Cast<DataItem>().ToList();
                break;
        }

        itemList.Init(currentInventory, list);
    }

    public void OnClickWeapon(bool check)
    {
        if (check)
        {
            SetInventoryType(InventoryTypes.Weapon);
        }
    }
    public void OnClickArmor(bool check)
    {
        if (check)
        {
            SetInventoryType(InventoryTypes.Armor);
        }
    }
    public void OnClickConsume(bool check)
    {
        if (check)
        {
            SetInventoryType(InventoryTypes.Consumable);
        }
    }

    // �׽�Ʈ�� ��ưŬ������ ���������� ���� ���ÿ� �̺�Ʈ ���� ���Ͽ� ��ϵǾ� ���ɼ���?
    public void OnAddItem()
    {
        itemPopUp.gameObject.SetActive(true);
        itemPopUp.isDelete = false;
        itemPopUp.init();
    }

    public void OnDeleteItem()
    {
        itemPopUp.gameObject.SetActive(true);
        itemPopUp.isDelete = true;
        itemPopUp.init();
    }

    //public void UseItem(DataCharacter character)
    //{
    //    itemList.OnUseItemMode(character);
    //}
}
