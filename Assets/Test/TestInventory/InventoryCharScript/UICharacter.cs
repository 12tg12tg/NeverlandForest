using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ��Ʈ�ѷ��� ����, View�� ������ �ϴ� Portrait�� Info, Select
// ���� ���� = Vars, ����ƽ�̶� ���⼱ ���� ������ �ȵ�����

public enum UserInput
{
    Default,
    UnEquip,
    Equip,
}

public class UICharacter : MonoBehaviour
{
    public DataCharacter currentChar; //  currentChar���� ���� ������ Vars�� ����?

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

    public void SetCharacter(DataCharacter dataCharacter, UserInput userInput = UserInput.Default)
    {
        switch (userInput)
        {
            case UserInput.Default:
                break;
            case UserInput.UnEquip:
                dataCharacter.dataWeapon = null;
                break;
            case UserInput.Equip:
                // �ϴ� ����
                break;
        }


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
