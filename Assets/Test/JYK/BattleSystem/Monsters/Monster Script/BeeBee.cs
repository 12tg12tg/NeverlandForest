using UnityEngine;
using System.Linq;

public class BeeBee : MonsterUnit
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>("0");
        Init(baseElem);
        Debug.Log($"{baseElem.Name}이 태어나다.");
        State = MonsterState.Idle;
    }

    public override void PlayAttackAnimation()
    {
        State = MonsterState.Attack;
        var rand = Random.Range(0, 1);
        switch (rand)
        {
            case 0:
                animator.SetTrigger("Bite");
                break;
            //case 1:
            //    animator.SetTrigger("Sting");
            //    break;
            //case 2:
            //    animator.SetTrigger("Projectile");
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

//if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName) &&
//            playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
//{
//    isAttackMotionEnd = true;
//}