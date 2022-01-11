using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// ����ڰ� � �Է��� ���� ��, �׿� �ش��ϴ� ������ �Ѱ��ְų�, ������ �����Ű�ų� �ϴ� Ŭ����

// ��Ʋ �ý��ۿ� �ʿ��� ������ �Ѱ��ְ�?, ��Ʋ �ý��ۿ��� ĳ���� ������ ���¿� ���� �� �Ѱ��ְ� ���

// ��Ʋ �ý����� �ǽð����� �÷��̾� ĳ���� ���¸� üũ�ϴ°� �ƴ� ĳ���Ϳ��� �ڽ��� ���º�ȭ�� ��Ʋ�� �˷��ִ¹��

public class PlayerBattleController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    /*
    ĳ���� ���� ��ų, �ɷ�ġ(����, ��������� ���� ��), ���� ��� � ���� ���� �� �� ����
    ĳ���� �������� ���� ( ������ �غ�, ������ ���, ���� )
    */

    // Instance
    public BattleManager manager;
    private List<UnitBase> targetUnit;

    // Component
    public PlayerStats stats; 
    public PlayerBattleFSM FSM;

    // Vars
    public PlayerCommand curCommand;
    public PlayerType playerType;    // Ȥ�� �ִϸ��̼��� ������ �ȴٸ� ��ģ�� �ν���� �ʱ�ȭ�� Ȯ���غ�����.

    // Property
    public PlayerStats Stats { get => stats; }

    public void TurnInit(PlayerCommand command)
    {
        // �÷��̾� ���� �Ǿ��� ��, ���� �غ� ���� �� ���� �ʱ�ȭ
        // �� ���ۿ� �ʿ��� �κе� ����

        curCommand = command;
        FSM.ChangeState(CharacterBattleState.Action);
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
        if (manager.FSM.curState != BattleState.Player)
            return;

        // ĳ���� Ŭ������ �� ������ ���۵�
        manager.OpenSkillUI(playerType);

        // ��Ʋ �ý��ۿ� ���� ���õ� ĳ���� ���� �����ֱ� PlayerStat
        // ���õ� ĳ������ ������ ���� ��ų����UI�� ��밡���� ��ų�� ��Ʋ�ý����� ����ְ�
        // ������ ��ų�� ����ϰų� �Ҽ� ����
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
