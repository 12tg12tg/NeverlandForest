//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//// 컨트롤러의 역할, View의 역할을 하는 Portrait과 Info, Select
//// 모델의 역할 = Vars, 스태틱이라 여기선 따로 선언은 안되있음

//public enum UserInput
//{
//    Default,
//    UnEquip,
//    Equip,
//}

//public class UICharacter : MonoBehaviour
//{
//    public DataCharacter currentChar; //  currentChar내부 값의 수정은 Vars만 가능?

//    public UICharacterSelect characterSelect;
//    public UICharacterPortrait characterPortrait;
//    public UICharacterInfo characterInfo;

//    public ItemObject weaponItem;

//    public UIWeaponSelect weaponSelectPanel;

//    private void Start()
//    {
//        characterSelect.Init(Vars.UserData.characterList);
//        SetCharacter(Vars.UserData.characterList[0]);
//    }

//    public void SetCharacter(DataCharacter dataCharacter, UserInput userInput = UserInput.Default)
//    {
//        switch (userInput)
//        {
//            case UserInput.Default:
//                break;
//            case UserInput.UnEquip:
//                dataCharacter.dataWeapon = null;
//                break;
//            case UserInput.Equip:
//                // 일단 보류
//                break;
//        }


//        ChangeCurChar(dataCharacter);
//        InitPortrait(dataCharacter);
//        InitInfo(dataCharacter);

//        weaponItem.Init(dataCharacter.dataWeapon);
//    }

//    public void InitPortrait(DataCharacter dataCharacter)
//    {
//        characterPortrait.Init(dataCharacter);
//    }

//    public void InitInfo(DataCharacter dataCharacter)
//    {
//        characterInfo.Init(dataCharacter);
//    }
//    public void ChangeCurChar(DataCharacter dataCharacter)
//    {
//        currentChar = dataCharacter;
//    }

//    //public void UseItem()
//    //{
//    //    var parent = GetComponentInParent<UIManager>();
//    //    parent.UseItem(currentChar);
//    //}

//    public void OpenWeaponList()
//    {
//        weaponSelectPanel.gameObject.SetActive(true);
//        weaponSelectPanel.Init(currentChar, Vars.UserData.weaponItemList);
//    }
//}
