using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookObject : MonoBehaviour
{
    private DataMaterial dataItem;
    public int Slot { get; set; }
    public Sprite icon;
    public TextMeshProUGUI nameText;
    public DataMaterial DataItem { get => dataItem; }

    public void Init(DataMaterial elem)
    {
        dataItem = elem;
        nameText.text = elem.ItemTableElem.name;
        icon = elem.ItemTableElem.IconSprite;
    }

}