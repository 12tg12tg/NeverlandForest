using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    // �ӽ�, ����� ������ Ÿ�� �����͸� ���� ������ ����Ʈ�� ������ �ֱ�
    public List<DataItem> equipItemList = new List<DataItem>();

    public List<SkillBase> ownSkillList = new List<SkillBase>();
}
