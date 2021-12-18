//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIInventoryItemInfo : MonoBehaviour
//{
//    public TextMeshProUGUI title;
//    public Image image;
//    public TextMeshProUGUI description;

//    public void SetInfo(DataWeapon data)
//    {
//        SetInfo(data.ItemTableElem);
//    }

//    public void SetInfo(DataCunsumable data)
//    {
//        SetInfo(data.ItemTableElem);
//    }

//    public void SetInfo(WeaponTableElem itemInfo)
//    {
//        title.text = itemInfo.name;
//        image.sprite = itemInfo.IconSprite;
//        description.text =
//            $"이름 : {itemInfo.name}\n" +
//            $"타입 : {itemInfo.type}\n" +
//            $"가격 : {itemInfo.cost}\n" +
//            $"무게 : {itemInfo.weight}\n" +
//            $"힘 : {itemInfo.stat_str}\n" +
//            $"건강 : {itemInfo.stat_con}\n" +
//            $"지능 : {itemInfo.stat_int}\n" +
//            $"운 : {itemInfo.stat_luk}\n" +
//            $"공격력 : {itemInfo.damage}\n" +
//            $"크리율 : {itemInfo.critical}\n" +
//            $"크리뎀 : {itemInfo.criticalDamage}\n";
//    }
//    public void SetInfo(ConsumableTableElem itemInfo)
//    {
//        title.text = itemInfo.name;
//        image.sprite = itemInfo.IconSprite;
//        description.text =
//            $"이름 : {itemInfo.name}\n" +
//            //$"타입 : {itemInfo.type}\n" +
//            $"가격 : {itemInfo.cost}\n" +
//            //$"무게 : {itemInfo.weight}\n" +
//            $"체력 : {itemInfo.hp}\n" +
//            $"마력 : {itemInfo.mp}\n" +
//            $"힘 : {itemInfo.statStr}\n" +
//            $"사용주기 : {itemInfo.duration}\n";
//    }
//}
