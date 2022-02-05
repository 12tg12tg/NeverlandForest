using System.Collections.Generic;
using UnityEngine;

public class DiarySkill : MonoBehaviour
{
    private static DiarySkill instance;
    public static DiarySkill Instance => instance;

   [Header("왼쪽 정보 창 연결")]
    public DiarySkillInfoUI info;
    [Header("우측 12개 아이콘 연결")]
    public List<DiarySkillButtonUi> skillButtons;

    public void Awake()
    {
        instance = this;
        SkillButtonInit();
    }
    public void SkillButtonInit() // 스킬아이콘 12개 세팅 : 태그 버튼 + 자동 활성화를 위한 함수
    {
        info.Init();
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
}
