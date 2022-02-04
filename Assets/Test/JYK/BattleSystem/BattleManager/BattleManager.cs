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

    [Header("플레이어 연결")]
    public PlayerBattleController boy;
    public PlayerBattleController girl;
    private PlayerCommand boyInput;
    private PlayerCommand girlInput;

    //Sub
    [Header("서브 링커 연결")]
    public BattleInput inputLink;
    public BattleWave waveLink;
    public BattleTile tileLink;
    public BattleUI uiLink;
    public BattleDirecting directLink;

    //Instance
    [Header("몬스터 관련 UI 캔버스 연결")]
    public Canvas uiCanvas;

    [Header("배틀 FSM 연결")]
    public BattleFSM FSM;

    //Vars
    private const int middleOfStage = 4;
    private Queue<MonsterCommand> monsterActionQueue = new Queue<MonsterCommand>();

    [Header("현재 턴 이전 턴 확인")]
    public int turn;
    public int preWaveTurn;

    [Header("현재 그룹 확인")]
    [SerializeField] private int curGroup;

    [Header("선공 / 후공 확인")]
    public bool isPlayerFirst;

    //Property
    public int Turn { get => turn; set => turn = value; }
    public bool IsWaitingTileSelect { get => tileLink.isWaitingTileSelect; }
    public Queue<MonsterCommand> MonsterActionQueue { get => monsterActionQueue; }
    public bool IsDuringPlayerAction { get; set; }

    private void Awake()
    {
        instance = this;
        boyInput = new PlayerCommand(boy, PlayerType.Boy);
        girlInput = new PlayerCommand(girl, PlayerType.Girl);

    }

    private void Start()
    {
        //Command 연결
        boy.command = boyInput;
        girl.command = girlInput;
    }

    // 초기화
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
                Debug.LogError($"보스전에 몬스터 후보가 {groups.Count}마리");

            // 보스전 - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            var rand = Random.Range(0, groups.Count);
            waveLink.wave2[0] = FindMonsterToId(groups[rand]);

            waveLink.wave2[1] = FindMonsterToId(bossIndex);       // 보스 중앙

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
            // 일반 배틀
            int randWaveCount = Random.Range(2, 4);
            List<MonsterUnit> temp = null;
            for (int i = 0; i < waveLink.curWave; i++)
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
    private MonsterUnit FindMonsterToId(int monsterId) // 지역 메소드
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
    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. Grade & Wave
        //  마지막방전투인지 일반전투인지를 판단하고,
        //  중반이 지나갔는지 아닌지를 판단하고,
        GradeWaveInit(isBlueMoonBattle, isEndOfDeongun);

        // 2. 변수 초기화
        VarInit();

        // 3. 몬스터 (랜덤뽑기) & 배치
        MonsterInit(isBlueMoonBattle, isEndOfDeongun);

        waveLink.SetWavePosition(waveLink.wave1);
        waveLink.SetWavePosition(waveLink.wave2);
        waveLink.SetWavePosition(waveLink.wave3);

        // 플레이어 스탯 전달받기
        // Vars 전역 저장소에서 불러오기.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;
        girl.stats.Hp = (int)Vars.UserData.uData.HerbalistHp;

        //플레이어 배치
        tileLink.SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        tileLink.SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //배틀상태 Start
        FSM.ChangeState(BattleState.Start);
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
        // 이겼는지 체크
        var monsterlist = monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if(monsterlist.Count == 0)
        {
            uiLink.PrintMessage($"승리!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
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
}
