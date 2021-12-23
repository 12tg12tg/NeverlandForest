//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UICharacterInfo : MonoBehaviour
//{
//    public TextMeshProUGUI charName;
//    public ScrollRect scrollRect;
//    public TextMeshProUGUI itemInfo;
//    public void Init(DataCharacter dataCharacter)
//    {
//        charName.text = dataCharacter.tableElem.name;

//        itemInfo.text =
//            $"체력 : {dataCharacter.Hp}\n" +
//            $"마력 : {dataCharacter.Mp}\n\n" +
//            $"물리공격력 : {dataCharacter.Ad}\n" +
//            $"마법공격력 : {dataCharacter.Ap}\n" +
//            $"방어력 : {dataCharacter.Defence}\n\n" +
//            $"힘 : {dataCharacter.Stat_str}\n" +
//            $"민첩 : {dataCharacter.Stat_dex}\n" +
//            $"지능 : {dataCharacter.Stat_int}\n" +
//            $"운 : {dataCharacter.Stat_luk}";

//        if (dataCharacter.dataWeapon != null)
//            itemInfo.text += $"\n무기 : {dataCharacter.dataWeapon.ItemTableElem.name}";

//        var list = dataCharacter.listDataArmor;
//        for (int idx = 0; idx < list.Count; ++idx)
//        {
//            itemInfo.text += $"\n방어구{idx + 1} : {list[idx].name}";
//        }
//    }
//}
