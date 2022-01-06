using UnityEngine;
using System.Linq;

public class BeeBee : MonsterUnit
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
        baseElem = DataTableManager.GetTable<MonsterTable>().GetData<MonsterTableElem>("0");
        Init(baseElem);
        Debug.Log($"{baseElem.name}이 태어나다.");
        State = MonsterState.Idle;
    }
    private void Update()
    {
    }

    public override void PlayAttackAnimation()
    {
        State = MonsterState.Attack;
        var rand = Random.Range(0, 1);
        switch (rand)
        {
            case 0:
                animator.SetTrigger("Projectile");
                break;
            //case 1:
            //    animator.SetTrigger("Sting");
            //    break;
            //case 2:
            //    animator.SetTrigger("Bite");
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
    // 공격 애니메이션 끝날때 태그 실행
    public override void TargetAttack()
    {
        var list = TileMaker.Instance.UnitOnTile(curCommand.target);
        var targetList = list.Cast<PlayerBattleController>().ToList();
        foreach(var target in targetList)
        {
            target.Stats.OnAttacked(this);
        }
        State = MonsterState.Idle;
    }

    public override void Move()
    {
        State = MonsterState.Move;
        var moveCount = Random.Range(1, 4);
        if(CurTile.TryGetFowardTile(out Tiles fowardTile, moveCount))
        {
            BattleManager.Instance.PlaceUnitOnTile(fowardTile.index, this, () => State = MonsterState.Idle , true);
        }
    }
}

//if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName) &&
//            playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
//{
//    isAttackMotionEnd = true;
//}