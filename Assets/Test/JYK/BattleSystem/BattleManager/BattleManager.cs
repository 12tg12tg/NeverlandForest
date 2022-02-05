using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum MonsterActionType { None, Attack, Move }
public enum PlayerType { None, Boy, Girl }
public enum ActionType { Skill, Item }


public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance { get => instance; }

    //Unit
    public List<MonsterUnit> monsters = new List<MonsterUnit>();

    [Header("�÷��̾� ����")]
    public PlayerBattleController boy;
    public PlayerBattleController girl;
    private PlayerCommand boyInput;
    private PlayerCommand girlInput;

    //Sub
    [Header("���� ��Ŀ ����")]
    public BattleInput inputLink;
    public BattleWave waveLink;
    public BattleTile tileLink;
    public BattleUI uiLink;
    public BattleDirecting directLink;
    public BattleDrag dragLink;

    //Instance
    [Header("���� ���� UI ĵ���� ����")]
    public Canvas uiCanvas;

    [Header("��Ʋ FSM ����")]
    public BattleFSM FSM;

    //Vars
    private const int middleOfStage = 4;
    private Queue<MonsterCommand> monsterActionQueue = new Queue<MonsterCommand>();

    [Header("���� �� ���� �� Ȯ��")]
    public int turn;
    public int preWaveTurn;

    [Header("���� �׷� Ȯ��")]
    [SerializeField] private int curGroup;

    [Header("���� / �İ� Ȯ��")]
    public bool isPlayerFirst;

    [Header("���� �׷� ����")]
    public CustomBattle customBattle;

    //Property
    public int Turn { get => turn; set => turn = value; }
    public bool IsWaitingTileSelect { get => tileLink.isWaitingTileSelect; }
    public Queue<MonsterCommand> MonsterActionQueue { get => monsterActionQueue; }
    public bool IsDuringPlayerAction { get; set; }

    private void Awake()
    {
        // ��Ʋ���� �ҷ�����
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Battle);

        instance = this;
        boyInput = new PlayerCommand(boy, PlayerType.Boy);
        girlInput = new PlayerCommand(girl, PlayerType.Girl);
    }

    private void Start()
    {
        //Command ����
        boy.command = boyInput;
        girl.command = girlInput;
    }

    // �ʱ�ȭ
    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. ���� �ʱ�ȭ
        VarInit();

        if (!customBattle.useCustomMode)
        {
            // 2. Grade & Wave
            //  ���������������� �Ϲ����������� �Ǵ��ϰ�,
            //  �߹��� ���������� �ƴ����� �Ǵ��ϰ�,
            GradeWaveInit(isBlueMoonBattle, isEndOfDeongun);

            // 3. ���� (�����̱�) & ��ġ
            MonsterInit(isBlueMoonBattle, isEndOfDeongun);
        }
        else
        {
            // 2~3. Custom Init
            CustomInit();
        }

        waveLink.SetWavePosition(waveLink.wave1);
        waveLink.SetWavePosition(waveLink.wave2);
        waveLink.SetWavePosition(waveLink.wave3);

        // �÷��̾� ���� ���޹ޱ�
        // Vars ���� ����ҿ��� �ҷ�����.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;

        //�÷��̾� ��ġ
        tileLink.SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        tileLink.SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //��Ʋ���� Start
        FSM.ChangeState(BattleState.Start);
    }

    private void CustomInit()
    {
        monsters.Clear();
        waveLink.wave1.Clear();
        waveLink.wave2.Clear();
        waveLink.wave3.Clear();
        for (int i = 0; i < 3; i++)
        {
            waveLink.wave1.Add(null);
            waveLink.wave2.Add(null);
            waveLink.wave3.Add(null);
        }

        List<MonsterUnit> realWave;
        List<bool> existList;
        List<MonsterPoolTag> customWave;
        waveLink.totalWave = customBattle.waveNum;

        for (int i = 0; i < waveLink.totalWave; i++)
        {
            if (i == 0)
            {
                realWave = waveLink.wave1;
                customWave = customBattle.cwave1;
                existList = customBattle.haveMonster1;
            }
            else if (i == 1)
            {
                realWave = waveLink.wave2;
                customWave = customBattle.cwave2;
                existList = customBattle.haveMonster2;
            }
            else
            {
                realWave = waveLink.wave3;
                customWave = customBattle.cwave3;
                existList = customBattle.haveMonster3;
            }
            for (int k = 0; k < 3; k++)
            {
                if (existList[k])
                {
                    realWave[k] = FindMonsterToId((int)customWave[k]);
                    realWave[k].Pos = new Vector2(k, 6);
                    realWave[k].SetActionCommand();
                }
            }
        }
    }

    private void GradeWaveInit(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        if (isBlueMoonBattle)
        {
            curGroup = 6;
            waveLink.totalWave = 3;
        }
        else if (isEndOfDeongun)
        {
            curGroup = Random.Range(4, 6);
            waveLink.totalWave = 3;
        }
        else
        {
            waveLink.totalWave = Random.Range(2, 4);

            var curCol = Vars.UserData.WorldMapPlayerData.currentIndex.y + 1; //1 ~ 9
            bool afterMiddle = curCol >= middleOfStage;

            if (!afterMiddle)
            {
                curGroup = Random.Range(0, 2);
            }
            else
            {
                curGroup = Random.Range(0, 4);
            }
        }
    }
    private void VarInit()
    {
        turn = 1;
        preWaveTurn = 1;
        waveLink.curWave = 0;
        IsDuringPlayerAction = false;
        GameManager.Manager.State = GameState.Battle;
    }

    private void MonsterInit(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        monsters.Clear();
        waveLink.wave1.Clear();
        waveLink.wave2.Clear();
        waveLink.wave3.Clear();
        for (int i = 0; i < 3; i++)
        {
            waveLink.wave1.Add(null);
            waveLink.wave2.Add(null);
            waveLink.wave3.Add(null);
        }

        var monsterElems = DataTableManager.GetTable<MonsterTable>().data.Values;
        var groups = (from n in monsterElems
                      where (n as MonsterTableElem).@group == curGroup
                      select int.Parse(n.id)).ToList();

        if (isBlueMoonBattle || isEndOfDeongun)
        {
            if (groups.Count != 3)
                Debug.LogError($"�������� ���� �ĺ��� {groups.Count}����");

            // ������ - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            var rand = Random.Range(0, groups.Count);
            waveLink.wave2[0] = FindMonsterToId(groups[rand]);

            waveLink.wave2[1] = FindMonsterToId(bossIndex);       // ���� �߾�

            rand = Random.Range(0, groups.Count);
            waveLink.wave2[2] = FindMonsterToId(groups[rand]);

            for (int i = 0; i < 3; i++)
            {
                waveLink.wave2[i].Pos = new Vector2(i, 6);
                waveLink.wave2[i].SetActionCommand();
            }

            // Wave1, Wave3
            List<MonsterUnit> temp;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    temp = waveLink.wave1;
                else
                    temp = waveLink.wave3;

                MakeNormalWave(groups, temp);
            }
        }
        else
        {
            // �Ϲ� ��Ʋ
            List<MonsterUnit> temp = null;
            for (int i = 0; i < waveLink.totalWave; i++)
            {
                if (i == 0)
                    temp = waveLink.wave1;
                else if (i == 1)
                    temp = waveLink.wave2;
                else
                    temp = waveLink.wave3;

                MakeNormalWave(groups, temp);
            }
        }
    }
    private MonsterUnit FindMonsterToId(int monsterId) // ���� �޼ҵ�
    {
        var tag = (MonsterPoolTag)monsterId;
        var go = MonsterPool.Instance.GetObject(tag);
        var unitSc = go.GetComponent<MonsterUnit>();
        unitSc.Init();
        return unitSc;
    }
    public void MakeNormalWave(List<int> idGroup, List<MonsterUnit> wave)
    {
        int monsterInWave = Random.Range(2, 4);
        int exceptCol = -1;
        if (monsterInWave == 2)
            exceptCol = Random.Range(0, 3);

        int rand;
        int colIndex = 0;
        for (int k = 0; k < monsterInWave; k++, colIndex++)
        {
            rand = Random.Range(0, idGroup.Count);
            if (colIndex == exceptCol)
                colIndex++;
            wave[colIndex] = FindMonsterToId(idGroup[rand]);
            wave[colIndex].Pos = new Vector2(colIndex, 6);
            wave[colIndex].SetActionCommand();
        }
    }

    //Command
    public void ClearCommand()
    {
        boyInput.Clear();
        girlInput.Clear();
    }

    public void DoCommand(PlayerType type, Vector2 target, DataPlayerSkill skill)
    {
        PlayerCommand command;
        PlayerBattleController attacker;
        if (type == PlayerType.Boy)
        {
            command = boyInput;
            attacker = boy;
        }
        else
        {
            command = girlInput;
            attacker = girl;
        }

        command.Create(target, skill);

        attacker.TurnInit(ActionType.Skill);
    }

    public void DoCommand(DataAllItem item)
    {
        PlayerCommand command;
        PlayerBattleController attacker;
        //if (type == PlayerType.Boy)
        //{
        //    command = boyInput;
        //    attacker = boy;
        //}
        //else
        //{
        //    command = girlInput;
        //    attacker = girl;
        //}

        //command.Create(target, item);

        //attacker.TurnInit(ActionType.Item);
    }

    public void EndOfPlayerAction()
    {
        // �̰���� üũ
        var monsterlist = monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if(monsterlist.Count == 0)
        {
            uiLink.PrintMessage($"�¸�!", 2.5f, () =>
                {
                    /*����â ����*/
                    SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
                    SceneManager.LoadScene("AS_RandomMap");
                });
        }
        else
        {
            IsDuringPlayerAction = false;
            if (boyInput.IsUpdated && girlInput.IsUpdated)
            {
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Boy, true);
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Girl, true);

                if (isPlayerFirst)
                {
                    FSM.ChangeState(BattleState.Monster);
                }
                else
                {
                    FSM.ChangeState(BattleState.Settlement);
                }
            }
            else
            {
                BottomUIManager.Instance.IsSkillLock = false;
            }
        }
    }

    public void AllMonsterDebuffCheck(UnityAction action = null)
    {
        var monster = monsters.Where(x => x.obsDebuffs != null)
                              .Select(x => x)
                              .ToList();

        for (int i = 0; i < monster.Count; i++)
        {
            monster[i].ObstacleHit();
        }

        action?.Invoke();
    }

    public AllItemTableElem GetCurrentArrowElem()
    {
        if (Vars.UserData.arrowType == ArrowType.Normal)
            return DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_20");
        else if (Vars.UserData.arrowType == ArrowType.Iron)
            return DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_21");
        else
            return null;
    }
}
