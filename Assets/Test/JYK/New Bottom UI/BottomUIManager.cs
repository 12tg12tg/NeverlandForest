using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class BottomUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static BottomUIManager instance;
    public static BottomUIManager Instance => instance;

    //Instance
    public BottomInfoUI info;
    public List<BottomSkillButtonUI> skillButtons;
    public List<GameObject> itemButtons;

    //Vars
    [HideInInspector] public ButtonState buttonState;

    private void Awake()
    {
        instance = this;
        SkillButtonInit();
    }

    public void SkillButtonInit()
    {
        info.Init();

        buttonState = ButtonState.Skill;
        itemButtons.ForEach((n) => n.SetActive(false));
        skillButtons.ForEach((n) => n.gameObject.SetActive(true));

        var list = Vars.BoySkillList;
        int count = list.Count;

        int buttonIndex = 1;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }

        buttonIndex = 7;
        list = Vars.GirlSkillList;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }
    }

    public void ItemButtonInit()
    {
        info.Init();

        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.SetActive(true));
        skillButtons.ForEach((n) => n.gameObject.SetActive(false));



    }
}
