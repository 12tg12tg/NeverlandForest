using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    public TileMaker tileMaker;
    private List<UnitBase> targetUnit;

    // Component
    public PlayerStats stats; 
    public PlayerBattleFSM FSM;

    // Vars
    public PlayerCommand command;
    public PlayerType playerType;    // Ȥ�� �ִϸ��̼��� ������ �ȴٸ� �ν����� �ʱ�ȭ�� Ȯ���� ��.

    // Property
    public PlayerStats Stats { get => stats; }

    private void Start()
    {
        tileMaker = TileMaker.Instance;
    }

    public void TurnInit(ActionType action)
    {
        FSM.ChangeState(CharacterBattleState.Action);
        manager.IsDuringPlayerAction = true;

        if (action == ActionType.Skill)
        {
            //��ų���
            StartCoroutine(CoActionCommand());
        }
        else
        {
            //�����ۻ��
        }
    }

    private IEnumerator CoActionCommand()
    {
        var tiles = tileMaker.GetSkillRangedTiles(command.target, command.skill.SkillTableElem.range);
        foreach (var tile in tiles)
        {
            tile.ConfirmAsTarget(command.type, tileMaker.LastClickPos, command.skill.SkillTableElem.range);
        }
        tileMaker.SetAllTileSoftClear();

        var playerActionState = FSM.GetState(CharacterBattleState.Action) as PlayerAction;
        yield return new WaitUntil(() => playerActionState.isAttackMotionEnd);
        playerActionState.isAttackMotionEnd = false;

        var monsterList = TileMaker.Instance.GetTargetList(command.target, command.skill.SkillTableElem.range);

        // ���Ÿ�� OnAttacked ���� -> �̶�, OnAttacked�� �ð��� �ɸ��� ������ �ʿ��Ұ�� ��ٷȴ� ���� �����ϴ� ��� ���
        foreach (var target in monsterList)
        {
            target.PlayHitAnimation();
            target.OnAttacked(command);
        }

        yield return new WaitForSeconds(1.5f);

        // ���ǻ� ���� �� ��Ʋ������ȯ �� ������ �¸� Ȯ��.
        foreach (var tile in tiles)
            tile.CancleConfirmTarget(playerType);

        FSM.ChangeState(CharacterBattleState.Idle); // �� unit�� ���°� �ٲ�� ��Ʋ������ ������Ʈ���� üũ�ϴٰ� ��������
        manager.EndOfPlayerAction();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager.FSM.curState != BattleState.Player)
            return;

        if (command.IsUpdated || manager.IsDuringPlayerAction)
            return;

    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
