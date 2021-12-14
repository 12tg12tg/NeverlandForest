using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacter : MonoBehaviour
{
    public DataCharacter currentChar;
    public UICharacterSelect characterSelect;
    public UICharacterPortrait characterPortrait;
    public UICharacterInfo characterInfo;

    public ItemObject weaponItem;

    public UIWeaponSelect weaponSelectPanel;

    private void Start()
    {
        characterSelect.Init(Vars.UserData.characterList);
        SetCharacter(Vars.UserData.characterList[0]);
    }

    public void SetCharacter(DataCharacter dataCharacter)
    {
        ChangeCurChar(dataCharacter);
        InitPortrait(dataCharacter);
        InitInfo(dataCharacter);

        weaponItem.Init(dataCharacter.dataWeapon);
    }

    public void InitPortrait(DataCharacter dataCharacter)
    {
        characterPortrait.Init(dataCharacter);
    }

    public void InitInfo(DataCharacter dataCharacter)
    {
        characterInfo.Init(dataCharacter);
    }
    public void ChangeCurChar(DataCharacter dataCharacter)
    {
        currentChar = dataCharacter;
    }

    //public void UseItem()
    //{
    //    var parent = GetComponentInParent<UIManager>();
    //    parent.UseItem(currentChar);
    //}

    public void OpenWeaponList()
    {
        weaponSelectPanel.gameObject.SetActive(true);
        weaponSelectPanel.Init(currentChar, Vars.UserData.weaponItemList);
    }
}
