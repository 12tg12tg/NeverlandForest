using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum MonsterState
{
    Idle, Attack, Move, DoSomething, Dead
}

public abstract class MonsterUnit : UnitBase, IAttackable
{
    // Vars
    private int sheild;
    private int speed;
    private MonsterType type;
    protected MonsterTableElem baseElem;
    public MonsterCommand command;
    private BattleManager manager;
    private bool isActionDone;
    private float delayTimer;
    private const float actionDelay = 0.5f;

    // Property
    public int Sheild { get => sheild; set => sheild = value; }
    public int Speed { get => speed; set => speed = value; }
    public bool IsBind { get; set; }
    public MonsterState State { get; set; }
    public MonsterType Type { get => type; }
    public MonsterTableElem BaseElem { get => baseElem; }

    // Update
    private void Update()
    {
        if(isActionDone)
        {
            delayTimer += Time.deltaTime;
            if(delayTimer > actionDelay)
            {
                delayTimer = 0f;
                isActionDone = false;
                State = MonsterState.Idle; // �� ������ �׼� ���� �˸��� ����.
            }
        }
    }

    // IAttackable
    public void OnAttacked(BattleCommand attacker)
    {
        var playerStats = attacker as PlayerCommand;
        var damage = playerStats.skill.SkillTableElem.damage;
        Debug.Log($"{Pos} ���Ͱ� {playerStats.type}���� {damage}�� ���ظ� �޴�. {Hp + damage} -> {Hp}");
        Hp -= damage;
        if (Hp <= 0)
        {
            PlayDeadAnimation();
            State = MonsterState.Dead;
        }
    }

    // �ʱ�ȭ
    public void Init(MonsterTableElem elem)
    {
        Hp = elem.hp;
        Atk = elem.atk;
        sheild = elem.sheild;
        speed = elem.speed;
        type = elem.type;
        manager ??= BattleManager.Instance;
        command ??= new MonsterCommand(this);
    }
    private void EraseThis()
    {
        CurTile.RemoveUnit(this);
        manager.monster.Remove(this);
    }


    // Action
    private bool CheckCanAttackPlayer()
    {
        // ���� ��Ÿ� ���� �÷��̾ �ִ��� �Ǵ�.
        var range = (int)type + 1;
        var dist = Pos.y;
        return dist <= range;
        // ���߿� �÷��̾ �������ִ��� �ƴ����� Ȯ��.
    }
    public MonsterCommand SetActionCommand()
    {
        // 1. ���� Ŀ�ǵ� �����
        command.Clear();

        // 2. �׾����� ���� Ȯ��
        if (State == MonsterState.Dead)
            return null;

        // 3. ������ ����� ��Ÿ����� �ִ��� Ȯ��. �ִٸ� ���� ��� ����.
        if (CheckCanAttackPlayer())
        {
            command.actionType = MonsterActionType.Attack;
            var randTarget = Random.Range(0, 2);
            command.target = randTarget == 0 ? manager.boy.Stats.Pos : manager.girl.Stats.Pos;
        }
        // 4. �ӹ� �������� Ȯ��
        else if(IsBind)
        {
            command.actionType = MonsterActionType.None;         
        }
        // 5. ������ ��� Ÿ�� ã��
        else
        {
            // ���� �࿡�� ���� Ȯ��
            var movableTiles = TileMaker.Instance.GetMovableTilesInSameRow(CurTile);
            int countInRow = movableTiles.Length;
            if (countInRow != 0)
            {
                var rand = Random.Range(0, countInRow);
                command.actionType = MonsterActionType.Move;
                command.target = movableTiles[rand].index;
            }
            else
            {
                // �ٸ� �࿡���� Ȯ��
                var movableTilesOtherRow = TileMaker.Instance.GetMovableTiles(CurTile);
                int count = movableTilesOtherRow.Length;
                if(count != 0)
                {
                    var rand = Random.Range(0, count);
                    command.actionType = MonsterActionType.Move;
                    command.target = movableTilesOtherRow[rand].index;
                }
                else
                {
                    command.actionType = MonsterActionType.None;
                }
            }
        }
        State = MonsterState.DoSomething;
        return command;
    }
    public void Move()
    {
        StartCoroutine(BattleManager.Instance.MoveUnitOnTile(command.target, this, () => isActionDone = true));
    }

    // Animation
    public abstract void PlayAttackAnimation();
    public abstract void PlayDeadAnimation();
    public abstract void PlayHitAnimation();


    // Animation Tag Function
    public void TargetAttack()
    {
        var list = TileMaker.Instance.GetUnitsOnTile(command.target);
        foreach (var target in list)
        {
            var player = target as PlayerStats;
            player.OnAttacked(command);
        }
        isActionDone = true;
    }
    public void StartSinking()
    {
        EraseThis();
        var dest = transform.position;
        dest.y -= 3f;
        StartCoroutine(Utility.CoTranslate(transform, transform.position, dest, 1f, () => gameObject.SetActive(false)));
    }
}
