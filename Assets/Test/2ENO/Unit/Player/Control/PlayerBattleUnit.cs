using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// ����ڰ� � �Է��� ���� ��, �׿� �ش��ϴ� ������ �Ѱ��ְų�, ������ �����Ű�ų� �ϴ� Ŭ����

// ��Ʋ �ý��ۿ� �ʿ��� ������ �Ѱ��ְ�?, ��Ʋ �ý��ۿ��� ĳ���� ������ ���¿� ���� �� �Ѱ��ְ� ���

// ��Ʋ �ý����� �ǽð����� �÷��̾� ĳ���� ���¸� üũ�ϴ°� �ƴ� ĳ���Ϳ��� �ڽ��� ���º�ȭ�� ��Ʋ�� �˷��ִ¹��

public class PlayerBattleUnit : UnitBase, IPointerClickHandler
{
    private List<UnitBase> targetUnit;

    // ĳ���� �������� ���� ( ������ �غ�, ������ ���, ���� )

    // ĳ���� ���� ��ų, �ɷ�ġ(����, ��������� ���� ��), ���� ��� � ���� ���� �� �� ����
    public PlayerBattleState playerState;

    private PlayerStats playerStat;

    public BattleManager battleSystem = BattleManager.Instance;

    public PlayerComand curCommand;

    public PlayerType playerType;

    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerBattleState>();
        playerStat = gameObject.GetComponent<PlayerStats>();
    }

    public void TurnInit(PlayerComand command)
    {
        // �÷��̾� ���� �Ǿ��� ��, ���� �غ� ���� �� ���� �ʱ�ȭ
        // �� ���ۿ� �ʿ��� �κе� ����

        curCommand = command;
        playerState.ChangeState(CharacterBattleState.Action);
    }

    public void AttackTarget(List<UnitBase> targetList)
    {
        // ��Ʋ �ý������κ��� Ÿ�� ����Ʈ�� �޾ƿͼ� �ش� Ÿ�ٵ鿡 ���� ����� ��ų�� ����
    }

    public void OnSkillUse(DataPlayerSkill playerSkill)
    {
        // ����ڰ� ��ų����UI Ŭ������ ��ų ���� �ش��ϴ� ��ų������ �޾ƿ��� �޼ҵ�

        // �̰����� ����� ��ų ������ ���� �ִϸ��̼� �� ����Ʈ ���� ���
        // �� Ÿ�� ���ֿ� ���� ����
        // �ִϸ��̼� �� ����Ʈ�� ��� ����Ǹ� ���� ����� �Ǵ��ؼ� �÷��̾� ���� �ٲ۴�
    }

    public void SendAttackEndMessage()
    {
        // ��Ʋ�ý��ۿ� ���� ĳ������ ������ ��� ����Ǿ����� �˷��ִ�
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
        if (playerState.curState != CharacterBattleState.Action)
            return;
        // ĳ���� Ŭ������ �� ������ ���۵�

        // ��Ʋ �ý��ۿ� ���� ���õ� ĳ���� ���� �����ֱ� PlayerStat
        // ���õ� ĳ������ ������ ���� ��ų����UI�� ��밡���� ��ų�� ��Ʋ�ý����� ����ְ�
        // ������ ��ų�� ����ϰų� �Ҽ� ����
    }
}
