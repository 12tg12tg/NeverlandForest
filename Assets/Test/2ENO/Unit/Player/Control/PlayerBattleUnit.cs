using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

// ����ڰ� � �Է��� ���� ��, �׿� �ش��ϴ� ������ �Ѱ��ְų�, ������ �����Ű�ų� �ϴ� Ŭ����

// ��Ʋ �ý��ۿ� �ʿ��� ������ �Ѱ��ְ�?, ��Ʋ �ý��ۿ��� ĳ���� ������ ���¿� ���� �� �Ѱ��ְ� ���

// ��Ʋ �ý����� �ǽð����� �÷��̾� ĳ���� ���¸� üũ�ϴ°� �ƴ� ĳ���Ϳ��� �ڽ��� ���º�ȭ�� ��Ʋ�� �˷��ִ¹��

public class PlayerBattleUnit : Observer, IPointerClickHandler
{
    public List<UnitBase> targetUnit;

    // ĳ���� �������� ���� ( ������ �غ�, ������ ���, ���� )
    public PlayerBattleFSM playerState;

    // ĳ���� ���� ��ų, �ɷ�ġ(����, ��������� ���� ��), ���� ��� � ���� ���� �� �� ����
    public PlayerStats playerStat;

    public BattleManager battleSystem;

    public bool isSelected = false;

    public override void Notify(ObservablePublisher publisher)
    {
        if (playerState.curState == CharacterBattleState.Death)
            return;

        var battleMgr = publisher as BattleManager;
        if(battleMgr.FSM.curState == BattleState.Player)
        {
            TurnInit();
        }
    }

    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerBattleFSM>();
        playerState.playerUnit = this;

        playerStat = gameObject.GetComponent<PlayerStats>();
    }

    public void TurnInit()
    {
        // �÷��̾� ���� �Ǿ��� ��, ���� �غ� ���� �� ���� �ʱ�ȭ
        // �� ���ۿ� �ʿ��� �κе� ����
        playerState.ChangeState(CharacterBattleState.AttackReady);
        isSelected = false;
    }

    public void SetAttackTarget(List<UnitBase> targetList)
    {
        // ��Ʋ �ý������κ��� Ÿ�� ����Ʈ�� �޾ƿͼ� �ش� Ÿ�ٵ鿡 ���� ����� ��ų�� ����
        targetUnit = targetList;
    }

    public void OnSkillUse(DataPlayerSkill playerSkill)
    {
        // ����ڰ� ��ų����UI Ŭ������ ��ų ���� �ش��ϴ� ��ų������ �޾ƿ��� �޼ҵ�

        // �̰����� ����� ��ų ������ ���� �ִϸ��̼� �� ����Ʈ ���� ���
        // �� Ÿ�� ���ֿ� ���� ����
        // �ִϸ��̼� �� ����Ʈ�� ��� ����Ǹ� ���� ����� �Ǵ��ؼ� �÷��̾� ���� �ٲ۴�


        // �ϴ� �ӽ÷� ��������ȯ, �� ���� ��������� �װɷ� ��ü
        var monsterList = targetUnit.Cast<MonsterStats>().ToList();

        if (monsterList.Count <= 0)
        {
            Debug.Log("Ÿ���� �����ϴ�");
        }
        else
        {
            // �ӽ÷� ���������ֱ�
            foreach (var monster in monsterList)
            {
                var UnitBase = new UnitBase();
                UnitBase.Atk = playerSkill.SkillTableElem.damage;
                monster.OnAttacked(UnitBase);
            }
        }

        playerState.SkillAttack();
        // ���� OnAttacked ������ Ÿ�� ���� �ֻ��� ��ü ���� GetComponents �ؼ� ��� �ҷ��ͼ� ��� ȣ��
    }


    // �� �ؿ� 4�� ������ �ϴ� ���..
    public void SendAttackEndMessage()
    {
        // ��Ʋ�ý��ۿ� ���� ĳ������ ������ ��� ����Ǿ����� �˷��ִ�
        battleSystem.PlayerAttackEnd();
    }

    public void SendSkillUseMessage()
    {
        // ��Ʋ �ý��ۿ� ���� ����� ��ų������ ������ �ش� ��ų�� ��Ÿ�� ���� �����ϰų� �Ҽ��ִ�.
    }

    public void SendSelectCharacterMessage()
    {
        // ĳ���� ���ý� ���õ� ĳ���� ���� ��Ʋ �ý��ۿ� �����ֱ�
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerState.curState != CharacterBattleState.AttackReady)
            return;
        // ĳ���� Ŭ������ �� ������ ���۵�

        // ��Ʋ �ý��ۿ� ���� ���õ� ĳ���� ���� �����ֱ� PlayerStat
        // ���õ� ĳ������ ������ ���� ��ų����UI�� ��밡���� ��ų�� ��Ʋ�ý����� ����ְ�
        // ������ ��ų�� ����ϰų� �Ҽ� ����
    }


}
