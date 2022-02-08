using UnityEngine;
using UnityEngine.UI;
public class CraftObj : MonoBehaviour
{
    public Image image;
    private string[] crafts;
    public string[] Crafts => crafts;
    private string time;
    public string Time => time;
    private string result;
    public string Result => result;
    private CraftIcon craftIcon;
    [SerializeField] private Button button;

    public void Init(CraftDataTable elem, string id, CraftIcon craftIcon)
    {
        this.craftIcon = craftIcon;
        result = elem.GetData<CraftTableElem>(id).result_ID;
        crafts = elem.GetCombination(result);
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        var stringid = $"ITEM_{result}";
        image.sprite = allitem.GetData<AllItemTableElem>(stringid).IconSprite;
        image.color = Color.white;
        time = elem.IsMakingTime(result);
        button.interactable = true;
    }

    public void ButtonOnClick()
    {
        craftIcon.currentCraft = this;
        craftIcon.OnChangedSelection();
    }
    public void Clear()
    {
        image.sprite = null;
        image.color = Color.clear;
        button.interactable = false;
    }
}
