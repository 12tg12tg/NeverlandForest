using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 팝업창
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
        // 여기서 직접 dataWeapon값 처리보단 SetCharacter를 통해 하는게 좋을수도!
        //selectedCharacter.dataWeapon = null;
        parent.SetCharacter(selectedCharacter, UserInput.UnEquip);
    }

    public void OnClickWeapon(DataWeapon dataWeapon)
    {
        selectedCharacter.dataWeapon = dataWeapon;
        parent.SetCharacter(selectedCharacter);
    }
}
