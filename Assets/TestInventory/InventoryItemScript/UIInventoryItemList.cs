using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

public enum WeaponOrder
{
    Default,
    Name,
    Type,
    Weight,
    Cost,
    Damage,
    Count,
}

public enum ArmorOrder
{
    Default,
    Name,
    Type,
    Weight,
    Cost,
    Def,
    Count,
}

public enum ConsumeOrder
{
    Default,
    Name,
    Type,
    Weight,
    Cost,
    Duration,
    Count,
}

public class UIInventoryItemList : MonoBehaviour
{
    public ItemObject itemPrefab;
    public ScrollRect scrollRect;

    public TMP_Dropdown dropdownSorting;
    public TMP_Dropdown dropdownFiltering;
    public TMP_Text textAccending;
    public Button useItemButton;

    public const int MaxItemCount = 100;
    private List<ItemObject> itemGoList = new List<ItemObject>();
    private List<DataItem> itemList = new List<DataItem>();

    private List<int> itemCountList = new List<int>();

    //private DataCharacter selectedCharacter;

    private bool sortingOptionAccending;

    private int selectedSlot = -1;
    private int currentFiltering;

    public InventoryTypes currentInventory;

    public UIInventoryItemInfo uiItemInfo;

    private void Awake()
    {
        for (var i = 0; i < MaxItemCount; ++i)
        {
            var item = Instantiate(itemPrefab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.SetActive(false);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnChangedSelection(item.Slot));
        }
    }

    public void Init(InventoryTypes invenType, List<DataItem> itemList)
    {
        currentInventory = invenType;
        sortingOptionAccending = true;
        var options = InitOption(invenType);
        dropdownSorting.options = options;
        textAccending.text = sortingOptionAccending ? "Accending" : "Deccending";

        this.itemList = itemList.Select(x => x).ToList();
        //switch (invenType)
        //{
        //    case InventoryTypes.Weapon:
        //        break;
        //    case InventoryTypes.Armor:
        //        break;
        //    case InventoryTypes.Consumable:
        //        break;
        //}
        options = InitFiltering(this.itemList);
        dropdownFiltering.options = options;

        dropdownFiltering.value = 0;
        dropdownSorting.value = 0;
        SetAllItems(invenType, this.itemList);
    }


    public List<TMP_Dropdown.OptionData> InitOption(InventoryTypes invenType)
    {
        var options = new List<TMP_Dropdown.OptionData>();
        switch (invenType)
        {
            case InventoryTypes.Weapon:
                for(int i=0; i < (int)WeaponOrder.Count; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData(((WeaponOrder)i).ToString()));
                }
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Default.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Name.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Type.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Weight.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Cost.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(WeaponOrder.Damage.ToString()));
                break;
            case InventoryTypes.Consumable:
                for (int i = 0; i < (int)ConsumeOrder.Count; i++)
                {
                    options.Add(new TMP_Dropdown.OptionData(((ConsumeOrder)i).ToString()));
                }
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Default.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Name.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Type.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Weight.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Cost.ToString()));
                //options.Add(new TMP_Dropdown.OptionData(ConsumeOrder.Duration.ToString()));
                break;
        }
        return options;
    }
    public List<TMP_Dropdown.OptionData> InitFiltering(List<DataItem> list)
    {
        var options = new List<TMP_Dropdown.OptionData>();
        var strList = new List<string>();

        options.Add(new TMP_Dropdown.OptionData("Default"));
        //foreach (var data in list)
        //{
        //    var elem = data.itemTableElem;
        //    if (!strList.Contains(elem.type))
        //    {
        //        options.Add(new TMP_Dropdown.OptionData(elem.type));
        //        strList.Add(elem.type);
        //    }
        //}
        return options;
    }

    public void SetAllItems(InventoryTypes invenType, List<DataItem> itemList, string filtering = "Default")
    {
        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }
        if (filtering.CompareTo("Default") == 0)
        {
            for (var i = 0; i < itemList.Count; ++i)
            {
                itemGoList[i].gameObject.SetActive(true);
                switch (invenType)
                {
                    case InventoryTypes.Weapon:
                        itemGoList[i].Init(itemList[i] as DataWeapon);
                        break;
                    case InventoryTypes.Consumable:
                        itemGoList[i].Init(itemList[i] as DataCunsumable);
                        break;
                }
            }
        }
        else
        {
            //for (var i = 0; i < itemList.Count; ++i)
            //{
            //    if (itemList[i].itemTableElem.type.Equals(filtering))
            //    {
            //        itemGoList[i].gameObject.SetActive(true);
            //        switch (invenType)
            //        {
            //            case InventoryTypes.Weapon:
            //                itemGoList[i].Init(itemList[i] as DataWeapon);
            //                break;
            //            case InventoryTypes.Consumable:
            //                itemGoList[i].Init(itemList[i] as DataCunsumable);
            //                break;
            //        }
            //    }
            //}
        }

        if (this.itemList.Count > 0)
        {
            selectedSlot = 0;
            EventSystem.current.SetSelectedGameObject(itemGoList[selectedSlot].gameObject);
            //OnChangedSelection(selectedSlot);
        }
    }



    public void SortConsumable(ConsumeOrder sortingOption, bool accending = true)
    {
        var consumalbeList = this.itemList.Cast<DataCunsumable>().ToList();

        switch (sortingOption)
        {
            case ConsumeOrder.Name:
                consumalbeList.Sort((lhs, rhs) => lhs.ItemTableElem.name.CompareTo(rhs.ItemTableElem.name));
                break;
            //case ConsumeOrder.Type:
            //    consumalbeList.Sort((lhs, rhs) => lhs.ItemTableElem.type.CompareTo(rhs.ItemTableElem.type));
            //    break;
            //case ConsumeOrder.Weight:
            //    consumalbeList.Sort((lhs, rhs) => lhs.ItemTableElem.weight.CompareTo(rhs.ItemTableElem.weight));
            //    break;
            case ConsumeOrder.Cost:
                consumalbeList.Sort((lhs, rhs) => lhs.ItemTableElem.cost.CompareTo(rhs.ItemTableElem.cost));
                break;
            case ConsumeOrder.Duration:
                consumalbeList.Sort((lhs, rhs) => lhs.ItemTableElem.duration.CompareTo(rhs.ItemTableElem.duration));
                break;
        }
        if (!accending)
        {
            consumalbeList.Reverse();
        }

        var list = consumalbeList.Cast<DataItem>().ToList();
        var filtering = dropdownFiltering.options[dropdownFiltering.value].text;
        SetAllItems(InventoryTypes.Consumable, list, filtering);
    }

    public void SortWeapon(WeaponOrder sortingOption, bool accending = true)
    {
        var weaponList = itemList.Cast<DataWeapon>().ToList();

        switch (sortingOption)
        {
            case WeaponOrder.Default:
                break;
            case WeaponOrder.Name:
                weaponList.Sort((lhs, rhs) => lhs.ItemTableElem.name.CompareTo(rhs.ItemTableElem.name));
                break;
            case WeaponOrder.Type:
                weaponList.Sort((lhs, rhs) => lhs.ItemTableElem.type.CompareTo(rhs.ItemTableElem.type));
                break;
            case WeaponOrder.Weight:
                weaponList.Sort((lhs, rhs) => lhs.ItemTableElem.weight.CompareTo(rhs.ItemTableElem.weight));
                break;
            case WeaponOrder.Cost:
                weaponList.Sort((lhs, rhs) => lhs.ItemTableElem.cost.CompareTo(rhs.ItemTableElem.cost));
                break;
            case WeaponOrder.Damage:
                weaponList.Sort((lhs, rhs) => lhs.ItemTableElem.damage.CompareTo(rhs.ItemTableElem.damage));
                break;
        }
        if (!accending)
        {
            weaponList.Reverse();
        }
        var list = weaponList.Cast<DataItem>().ToList();
        var filtering = dropdownFiltering.options[dropdownFiltering.value].text;
        SetAllItems(InventoryTypes.Weapon, list, filtering);
    }

    public void OnChangedSelection(int slot)
    {
        switch (currentInventory)
        {
            case InventoryTypes.Weapon:
                uiItemInfo.SetInfo(itemGoList[slot].DataItem as DataWeapon);
                break;
            case InventoryTypes.Consumable:
                uiItemInfo.SetInfo(itemGoList[slot].DataItem as DataCunsumable);
                break;
        }
        selectedSlot = slot;
    }

    public void OnChangedSortingOption(int index)
    {
        switch (currentInventory)
        {
            case InventoryTypes.Weapon:
                SortWeapon((WeaponOrder)index, sortingOptionAccending);
                break;
            case InventoryTypes.Consumable:
                SortConsumable((ConsumeOrder)index, sortingOptionAccending);
                break;
        }
    }

    public void OnClickAccending()
    {
        sortingOptionAccending = !sortingOptionAccending;
        textAccending.text = sortingOptionAccending ? "Accending" : "Deccending";
        OnChangedSortingOption(dropdownSorting.value);
    }

    public void OnChangedFiltering(int index)
    {
        SetAllItems(currentInventory, itemList, dropdownFiltering.options[index].text);
    }

    //public void OnUseItemMode(DataCharacter character)
    //{
    //    useItemButton.gameObject.SetActive(true);
    //    selectedCharacter = character;
    //}

    //public void OnUseItem()
    //{
    //    itemCountList[selectedSlot]--;
    //    itemGoList[selectedSlot].number.text = itemCountList[selectedSlot].ToString();


    //    useItemButton.gameObject.SetActive(false);
    //    Debug.Log($"{selectedCharacter.tableElem.name}이/가 {itemGoList[selectedSlot].text.text}를 사용했습니다.");
    //    Debug.Log($"{itemGoList[selectedSlot].text.text}의 남은 갯수 : {itemCountList[selectedSlot]}");

    //    var parent = GetComponentInParent<UIManager>();
    //    parent.BackCharacter();
    //}


    //public List<T> ChangeListType<T>(List<DataTableElemBase> list) where T : DataTableElemBase
    //{
    //    var newlist = new List<T>();
    //    foreach (var elem in list)
    //    {
    //        newlist.Add(elem as T);
    //    }
    //    return newlist;
    //}

    //public List<DataTableElemBase> ChangeListType<T>(List<T> list) where T : DataTableElemBase
    //{
    //    var newlist = new List<DataTableElemBase>();
    //    foreach (var elem in list)
    //    {
    //        newlist.Add(elem);
    //    }
    //    return newlist;
    //}
}
