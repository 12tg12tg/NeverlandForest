using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CraftObj : MonoBehaviour
{
    public int Slot { get; set; }
    public Image image;
    private string[] crafts;
    public string[] Crafts => crafts;
    private string time;
    public string Time => time;
    private string result;
    public string Result => result;
    public void Init(CraftDataTable elem, string id)
    {
        result = elem.GetData<CraftTableElem>(id).result_ID;
        crafts = elem.GetCombination(result);
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var stringid = $"ITEM_{result}";
        image.sprite = allitem.GetData<AllItemTableElem>(stringid).IconSprite;
        time = elem.IsMakingTime(result);
    }
}
