using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTestMaker : MonoBehaviour
{
    private string result = string.Empty;
    // Start is called before the first frame update
    void Start()
    {
        //제조 조합식 테스트
        var craftTable = DataTableManager.GetTable<CraftDataTable>();
        var userCraftData = Vars.UserData.HaveCraftIDList;
        craftTable.IsCombine(1.ToString(), 9.ToString(), out result);
        var craftid1 = craftTable.GetCraftId(result);
        Debug.Log($"1번째{result}");

        if (!userCraftData.Contains(craftid1))
        {
            userCraftData.Add(craftid1);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(1.ToString(), 9.ToString(), out result,1.ToString());
        var craftid2 = craftTable.GetCraftId(result);
        Debug.Log($"2번째{result}");

        if (!userCraftData.Contains(craftid2))
        {
            userCraftData.Add(craftid2);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(1.ToString(), 0.ToString(), out result);
        var craftid3 = craftTable.GetCraftId(result);
        Debug.Log($"3번째{result}");

        if (!userCraftData.Contains(craftid3))
        {
            userCraftData.Add(craftid3);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(2.ToString(), 0.ToString(), out result);
        var craftid4 = craftTable.GetCraftId(result);
        Debug.Log($"4번째{result}");

        if (!userCraftData.Contains(craftid4))
        {
            userCraftData.Add(craftid4);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(2.ToString(), 9.ToString(), out result);
        var craftid5 = craftTable.GetCraftId(result);
        Debug.Log($"5번째{result}");

        if (!userCraftData.Contains(craftid5))
        {
            userCraftData.Add(craftid5);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(5.ToString(), 5.ToString(), out result);
        var craftid6 = craftTable.GetCraftId(result);
        Debug.Log($"6번째{result}");

        if (!userCraftData.Contains(craftid6))
        {
            userCraftData.Add(craftid6);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(14.ToString(), 14.ToString(), out result,9.ToString());
        var craftid7 = craftTable.GetCraftId(result);
        Debug.Log($"7번째{result}");

        if (!userCraftData.Contains(craftid7))
        {
            userCraftData.Add(craftid7);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(1.ToString(), 2.ToString(), out result, 2.ToString());
        var craftid8 = craftTable.GetCraftId(result);
        Debug.Log($"8번째{result}");

        if (!userCraftData.Contains(craftid8))
        {
            userCraftData.Add(craftid8);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(16.ToString(), 16.ToString(), out result, 9.ToString());
        var craftid9 = craftTable.GetCraftId(result);
        Debug.Log($"9번째{result}");

        if (!userCraftData.Contains(craftid9))
        {
            userCraftData.Add(craftid9);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(3.ToString(), 3.ToString(), out result, 3.ToString());
        var craftid10 = craftTable.GetCraftId(result);
        Debug.Log($"10번째{result}");

        if (!userCraftData.Contains(craftid10))
        {
            userCraftData.Add(craftid10);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
        craftTable.IsCombine(4.ToString(), 4.ToString(), out result,4.ToString());
        var craftid11 = craftTable.GetCraftId(result);
        Debug.Log($"11번째{result}");

        if (!userCraftData.Contains(craftid11))
        {
            userCraftData.Add(craftid11);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Craft);
        }
    }
}
