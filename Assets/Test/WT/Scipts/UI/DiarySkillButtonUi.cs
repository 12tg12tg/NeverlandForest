using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiarySkillButtonUi : MonoBehaviour
{
    public DataPlayerSkill skill;                       // ����ִ� ��ų����
    [SerializeField] private Image skillImg;

    public Image SkillImg
    {
        get => skillImg;
    }

    public void Init(DataPlayerSkill skill) // ��ų�� ���� ��ư �ʱ�ȭ
    {
        if (skill == null)
            return;

        this.skill = skill;
        skillImg.sprite = skill.SkillTableElem.IconSprite;
    }
    public void IntoSkillStage() // ��ư
    {
        DiarySkill.Instance.info.Init(skill);
    }
}
