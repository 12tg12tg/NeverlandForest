using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterPortrait : MonoBehaviour
{
    public Image image;

    public void Init(DataCharacter data)
    {
        image.sprite = data.tableElem.ProfileSprite;
        image.SetNativeSize();
    }
}
