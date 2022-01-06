using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushMush : MonsterUnit
{
    private Animator animator;
    private MonsterTableElem baseElem;
    public MonsterTableElem BaseElem { get => baseElem; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>("1");
        Init(baseElem);
        Debug.Log($"{baseElem.name}�� �¾��.");
    }

    public override void PlayAttackAnimation()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayDeadAnimation()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayHitAnimation()
    {
        throw new System.NotImplementedException();
    }
}
