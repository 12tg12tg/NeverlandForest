using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillConnection
{
    public bool isAnimationDone { get; set; }
    /// <summary>
    /// isAnimationDone�� �ݵ�� true��
    /// </summary>
    public void DoSkill();
}
