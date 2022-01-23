using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillConnection
{
    public bool isAnimationDone { get; set; }
    /// <summary>
    /// isAnimationDone을 반드시 true로
    /// </summary>
    public void DoSkill();
}
