using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ����ڰ� � �Է��� ���� ��, �׿� �ش��ϴ� ������ �Ѱ��ְų�, ������ �����Ű�ų� �ϴ� Ŭ����
// ��Ʋ �ý��ۿ� �ʿ��� ������ �Ѱ��ְ�, ��Ʋ �ý��ۿ��� ĳ���� ������ ���¿� ���� �� �Ѱ��ְ� ���
// ��Ʋ �ý����� �ǽð����� �÷��̾� ĳ���� ���¸� üũ�ϴ°� �ƴ� ĳ���Ϳ��� �ڽ��� ���º�ȭ�� ��Ʋ�� �˷��ִ¹��

public class PlayerBattleController : MonoBehaviour, IDropHandler
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

    // Animation
    public void PlayIdleAnimation()
    {
        FSM.animator.SetTrigger("Idle");
    }
    public void PlayHitAnimation()
    {
        FSM.animator.SetTrigger("Hit");
    }

    public void PlayDeadAnimation()
    {
        FSM.animator.SetTrigger("Dead");
    }

    public void PlayWinAnimation()
    {
        FSM.animator.SetTrigger("Win");
    }

    // �ڽ��� ��ų ���
    public void TurnInit(ActionType action, bool isDrag)
    {
        FSM.ChangeState(CharacterBattleState.Action); // Action Init���� ���� �ִϸ��̼� ��� ����
        manager.IsDuringPlayerAction = true;

        if (action == ActionType.Skill)
        {
            //��ų���
            StartCoroutine(CoActionCommand(isDrag));
        }
        else
        {
            //�����ۻ��
        }
    }

    private IEnumerator CoActionCommand(bool isDrag)
    {
        // �Է� ��� ����
        manager.inputLink.isLastInputDrag = isDrag;

        // ��ŵ��ư ��� ��Ȱ��ȭ
        manager.uiLink.turnSkipButton.interactable = false;

        //��ų ���� �ٴ� Ÿ�� ǥ��
        var tiles = tileMaker.GetSkillRangedTiles(command.target, command.skill.SkillTableElem.range);
        foreach (var tile in tiles)
        {
            if(isDrag)
                tile.ConfirmAsTarget(command.type, manager.dragLink.lastDragWorldPos, command.skill.SkillTableElem.range);
            else
                tile.ConfirmAsTarget(command.type, tileMaker.LastClickPos, command.skill.SkillTableElem.range);
        }
        tileMaker.SetAllTileSoftClear();

        if (command.skill.SkillTableElem.id == manager.costLink.skillID_chargeOil) // ���� ����
        {
            // ���� �ִ� ������ PlayerAction�� MotionEnd ���� �� ���� ��ٸ���.
            var playerActionState = FSM.GetState(CharacterBattleState.Action) as PlayerAction;
            yield return new WaitUntil(() => playerActionState.isAttackMotionEnd);
            playerActionState.isAttackMotionEnd = false;

            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            //���� ���� �غ�. �ݶ��̴� �ѱ�
            var monsterList = TileMaker.Instance.GetTargetList(command.target, command.skill.SkillTableElem.range, isDrag);
            var neverChangeMonsterList = new List<MonsterUnit>(monsterList);
            foreach (var target in neverChangeMonsterList)
            {
                target.triggerLinker.EnableHitTrigger();
            }

            //���� �°� PlayerAction�� MotionEnd ���� �� ���� ��ٸ���.
            var playerActionState = FSM.GetState(CharacterBattleState.Action) as PlayerAction;
            yield return new WaitUntil(() => playerActionState.isAttackMotionEnd);
            playerActionState.isAttackMotionEnd = false;

            // ���Ÿ�� OnAttacked ���� -> �̶�, OnAttacked�� �ð��� �ɸ��� ������ �ʿ��Ұ�� ��ٷȴ� ���� �����ϴ� ��� ���
            if (command.skill.SkillTableElem.id != manager.costLink.skillID_focusAttack) // ���߰��ݾƴ� ��
            {
                foreach (var target in neverChangeMonsterList)
                {
                    target.PlayHitAnimation();
                    target.OnAttacked(command);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }

        // �Ҹ� �� �Ҹ�
        if (playerType == PlayerType.Boy)
        {
            manager.costLink.CostArrow(command.skill);
        }
        else
        {
            manager.costLink.CostLanternOrOil(command.skill);
        }
        BottomUIManager.Instance.UpdateCostInfo();

        // ���ǻ� ���� �� ��Ʋ������ȯ �� ������ �¸� Ȯ��.
        foreach (var tile in tiles)
            tile.CancleConfirmTarget(playerType);

        FSM.ChangeState(CharacterBattleState.Idle); // �� unit�� ���°� �ٲ�� ��Ʋ������ ������Ʈ���� üũ�ϴٰ� ��������
        if(!manager.isTutorial)
            manager.uiLink.turnSkipButton.interactable = true;
        manager.EndOfPlayerAction();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Pointer is drop here to {stats.Pos} Tile! ");
        TileMaker.Instance.LastDropPos = stats.Pos;
    }
}
