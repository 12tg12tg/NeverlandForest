using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterInfo : MonoBehaviour
{
    public TextMeshProUGUI charName;
    public ScrollRect scrollRect;
    public TextMeshProUGUI itemInfo;
    public void Init(DataCharacter dataCharacter)
    {
        charName.text = dataCharacter.tableElem.name;

        itemInfo.text =
            $"ü�� : {dataCharacter.Hp}\n" +
            $"���� : {dataCharacter.Mp}\n\n" +
            $"�������ݷ� : {dataCharacter.Ad}\n" +
            $"�������ݷ� : {dataCharacter.Ap}\n" +
            $"���� : {dataCharacter.Df}\n\n" +
            $"�� : {dataCharacter.StatStr}\n" +
            $"��ø : {dataCharacter.StatDex}\n" +
            $"���� : {dataCharacter.StatInt}\n" +
            $"�� : {dataCharacter.StatLuk}";

        if (dataCharacter.dataWeapon != null)
            itemInfo.text += $"\n���� : {dataCharacter.dataWeapon.ItemTableElem.name}";

        var list = dataCharacter.listDataArmor;
        for (int idx = 0; idx < list.Count; ++idx)
        {
            itemInfo.text += $"\n��{idx + 1} : {list[idx].name}";
        }
    }
}
