using System.Collections.Generic;
using UnityEngine;

public class DiarySkill : MonoBehaviour
{
    private static DiarySkill instance;
    public static DiarySkill Instance => instance;

    [Header("���� ���� â ����")]
    public DiarySkillInfoUI info;
    [Header("���� 12�� ������ ����")]
    public List<DiarySkillButtonUi> skillButtons;

    public void Awake()
    {
        instance = this;
    }
    public void SkillButtonInit() // ��ų������ 12�� ���� : �±� ��ư + �ڵ� Ȱ��ȭ�� ���� �Լ�
    {
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
        info.Init();
    }
}
