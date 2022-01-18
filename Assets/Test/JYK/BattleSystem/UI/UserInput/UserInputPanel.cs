using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete("사용하지 않게된 UI.")]
public class UserInputPanel : MonoBehaviour
{
    public InputPairImage firstInput;
    public InputPairImage secondInput;

    public InputPairImage dragSlot;

    public bool IsBothInputPresent 
    { 
        get => firstInput.gameObject.activeInHierarchy &&
            secondInput.gameObject.activeInHierarchy; 
    }

    public void Init(PlayerCommand first, PlayerCommand second)
    {
        if (first == null)
            return;

        firstInput.gameObject.SetActive(true);
        if (first.actionType == ActionType.Skill)
            firstInput.Init(first.type, first.skill);
        else
            firstInput.Init(first.type, first.item);

        if (second == null)
            return;

        secondInput.gameObject.SetActive(true);
        if (second.actionType == ActionType.Skill)
            secondInput.Init(second.type, second.skill);
        else
            secondInput.Init(second.type, second.item);
    }

    public void Clear()
    {
        firstInput.gameObject.SetActive(false);
        secondInput.gameObject.SetActive(false);
    }

    public void Swap()
    {
        InputPairImage temp = firstInput;
        firstInput = secondInput;
        secondInput = temp;
    }
}
