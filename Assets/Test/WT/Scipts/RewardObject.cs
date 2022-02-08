using UnityEngine;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
    private DataAllItem item;
    public DataAllItem Item { get => item; set => item = value; }
    private bool isSelect;
    public Image selectedImg;

    public Sprite selectedImage;

    public bool IsSelect
    {
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                selectedImg.color = Color.white;
                selectedImg.sprite = selectedImage;
            }
            else
            {
                selectedImg.color = Color.clear;
                selectedImg.sprite = null;
            }

        }
    }

    public void ItemButtonClick()
    {
        if (item == null)
            return;
        IsSelect = !IsSelect;
    }
}
