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
            $"�� : {itemInfo.stat_str}\n" +
            $"�ǰ� : {itemInfo.stat_con}\n" +
            $"���� : {itemInfo.stat_int}\n" +
            $"�� : {itemInfo.stat_luk}\n" +
            $"���ݷ� : {itemInfo.damage}\n" +
            $"ũ���� : {itemInfo.critical}\n" +
            $"ũ���� : {itemInfo.criticalDamage}\n";
    }
    public void SetInfo(ConsumableTableElem itemInfo)
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
            $"�� : {itemInfo.statStr}\n" +
            $"����ֱ� : {itemInfo.duration}\n";
    }
}
