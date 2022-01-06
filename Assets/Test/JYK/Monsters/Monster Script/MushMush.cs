using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushMush : MonsterUnit
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>("1");
        Init(baseElem);
        Debug.Log($"{baseElem.name}이 태어나다.");
    }

    public override void PlayAttackAnimation()
    {
        var rand = Random.Range(0, 1);
        switch (rand)
        {
            case 0:
                animator.SetTrigger("Punch");
                break;
            //case 1:
            //    animator.SetTrigger("Projectile");
            //    break;
            //case 2:
            //    animator.SetTrigger("Spell");
            //    break;
        }
    }

    public override void PlayDeadAnimation()
    {
        animator.SetTrigger("Die");
    }

    public override void PlayHitAnimation()
    {
        animator.SetTrigger("Damaged");
    }

}
