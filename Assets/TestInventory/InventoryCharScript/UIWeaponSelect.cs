using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponSelect : MonoBehaviour
{
    public UICharacter parent;

    public ItemObject itemPrefab;
    private List<ItemObject> itemGoList = new List<ItemObject>();

    public ScrollRect scrollRect;

    private DataCharacter selectedCharacter;

    public void Awake()
    {
        for (var i = 0; i < UIInventoryItemList.MaxItemCount; ++i)
        {
            var item = Instantiate(itemPrefab, scrollRect.content);
            item.Slot = i;
            itemGoList.Add(item);
            item.gameObject.SetActive(false);
            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickWeapon(item.DataItem as DataWeapon));
        }
    }

    public void Init(DataCharacter dataCharacter, List<DataWeapon> weaponList)
    {
        selectedCharacter = dataCharacter;

        foreach (var item in itemGoList)
        {
            item.gameObject.SetActive(false);
        }

        for (var i = 0; i < weaponList.Count; ++i)
        {
            itemGoList[i].gameObject.SetActive(true);
            itemGoList[i].Init(weaponList[i]);
        }
    }

    public void OnClicnUnEquip()
    {
        selectedCharacter.dataWeapon = null;
        parent.SetCharacter(selectedCharacter);
    }

    public void OnClickWeapon(DataWeapon dataWeapon)
    {
        selectedCharacter.dataWeapon = dataWeapon;
        parent.SetCharacter(selectedCharacter);
    }
}
