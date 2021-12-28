using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    // 임시, 포장된 아이템 타입 데이터를 장착 아이템 리스트로 가지고 있기
    public List<DataItem> equipItemList = new List<DataItem>();

    public List<SkillBase> ownSkillList = new List<SkillBase>();
}
