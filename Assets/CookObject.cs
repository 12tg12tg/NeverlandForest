using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookObject : MonoBehaviour
{
    public int Slot { get; set; }
    public Sprite icon;
    public TextMeshProUGUI nameText;
    private DataMaterial dataItem;
    public DataMaterial DataItem { get => dataItem; }

    public void Init(DataMaterial elem)
    {
        dataItem = elem;
        nameText.text = elem.ItemTableElem.name;
        icon = elem.ItemTableElem.IconSprite;
    }

}