using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ����ڰ� � �Է��� ���� ��, �׿� �ش��ϴ� ������ �Ѱ��ְų�, ������ �����Ű�ų� �ϴ� Ŭ����
// ��Ʋ �ý��ۿ� �ʿ��� ������ �Ѱ��ְ�, ��Ʋ �ý��ۿ��� ĳ���� ������ ���¿� ���� �� �Ѱ��ְ� ���
// ��Ʋ �ý����� �ǽð����� �÷��̾� ĳ���� ���¸� üũ�ϴ°� �ƴ� ĳ���Ϳ��� �ڽ��� ���º�ȭ�� ��Ʋ�� �˷��ִ¹��

public class PlayerBattleController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    /* ĳ���� ���� ��ų, �ɷ�ġ(����, ��������� ���� ��), ���� ��� � ���� ���� �� �� ����
    ĳ���� �������� ���� ( ������ �غ�, ������ ���, ���� ) */


    // Instance
    public BattleManager manager;
    private List<UnitBase> targetUnit;

    // Component
    public PlayerStats stats; 
    public PlayerBattleFSM FSM;

    // Vars
    public PlayerCommand command;
    public PlayerType playerType;    // Ȥ�� �ִϸ��̼��� ������ �ȴٸ� �ν����� �ʱ�ȭ�� Ȯ���� ��.

    // Property
    public PlayerStats Stats { get => stats; }

    public void TurnInit()
    {
        FSM.ChangeState(CharacterBattleState.Action);
        manager.IsDuringPlayerAction = true;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager.FSM.curState != BattleState.Player)
            return;

        if (command.IsUpdated || manager.IsDuringPlayerAction)
            return;

        // ĳ���� Ŭ������ �� ������ ���۵�
        manager.OpenSkillUI(playerType);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
