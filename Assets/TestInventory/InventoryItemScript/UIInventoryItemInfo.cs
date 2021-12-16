using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItemInfo : MonoBehaviour
{
    public TextMeshProUGUI title;
    public Image image;
    public TextMeshProUGUI description;

    public void SetInfo(DataWeapon data)
    {
        SetInfo(data.ItemTableElem);
    }

    public void SetInfo(DataCunsumable data)
    {
        SetInfo(data.ItemTableElem);
    }

    public void SetInfo(WeaponTableElem itemInfo)
    {
        title.text = itemInfo.name;
        image.sprite = itemInfo.IconSprite;
        description.text =
            $"�̸� : {itemInfo.name}\n" +
            $"Ÿ�� : {itemInfo.type}\n" +
            $"���� : {itemInfo.cost}\n" +
            $"���� : {itemInfo.weight}\n" +
            $"�� : {itemInfo.str}\n" +
            $"�ǰ� : {itemInfo.con}\n" +
            $"���� : {itemInfo.intellet}\n" +
            $"�� : {itemInfo.luck}\n" +
            $"���ݷ� : {itemInfo.damage}\n" +
            $"ũ���� : {itemInfo.critRate}\n" +
            $"ũ���� : {itemInfo.critDamage}\n";
    }
    public void SetInfo(ConsumableTableElemInho itemInfo)
    {
        title.text = itemInfo.name;
        image.sprite = itemInfo.IconSprite;
        description.text =
            $"�̸� : {itemInfo.name}\n" +
            //$"Ÿ�� : {itemInfo.type}\n" +
            $"���� : {itemInfo.cost}\n" +
            //$"���� : {itemInfo.weight}\n" +
            $"ü�� : {itemInfo.hp}\n" +
            $"���� : {itemInfo.mp}\n" +
            $"�� : {itemInfo.str}\n" +
            $"����ֱ� : {itemInfo.duration}\n";
    }
}
