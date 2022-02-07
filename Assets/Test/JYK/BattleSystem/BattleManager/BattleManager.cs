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
    public BattleDirecting directingLink;
    public BattleDrag dragLink;
    public BattleCost costLink;

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

    [Header("몬스터 그룹 설정")]
    public CustomBattle customBattle;

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
    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. 변수 초기화
        VarInit();

        if (!customBattle.useCustomMode)
        {
            // 2. Grade & Wave
            //  마지막방전투인지 일반전투인지를 판단하고,
            //  중반이 지나갔는지 아닌지를 판단하고,
            GradeWaveInit(isBlueMoonBattle, isEndOfDeongun);

            // 3. 몬스터 (랜덤뽑기) & 배치
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

        // 플레이어 스탯 전달받기
        // Vars 전역 저장소에서 불러오기.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;

        //플레이어 배치
        tileLink.SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        tileLink.SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //배틀상태 Start
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

        // 화살
        DataAllItem temp;
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.arrowElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.arrowElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.arrowNum;
        Vars.UserData.AddItemData(temp);

        // 쇠화살
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.ironArrowElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.ironArrowElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.ironArrowNum;
        Vars.UserData.AddItemData(temp);

        // 오일
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.oilElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.oilElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.oilNum;
        Vars.UserData.AddItemData(temp);

        // 랜턴밝기
        ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
        ConsumeManager.FullingLantern(customBattle.lanternCount);
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

    public void EndOfPlayerAction()
    {
        // 이겼는지 체크
        var monsterlist = monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if(monsterlist.Count == 0)
        {
            if (waveLink.IsAllWaveClear())
            {
                uiLink.turnSkipButton.SetActive(false);
                uiLink.progressTrans.SetActive(false);
                uiLink.PrintMessage($"승리!", 2.5f, () =>
                    {
                    /*보상창 띄우기*/
                        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
                        SceneManager.LoadScene("AS_RandomMap");
                    });
            }
            else
            {
                IsDuringPlayerAction = false;
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Boy, true);
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Girl, true);
                if (isPlayerFirst)
                {
                    FSM.ChangeState(BattleState.Monster);
                    uiLink.turnSkipButton.SetActive(false);
                }
                else
                {
                    FSM.ChangeState(BattleState.Settlement);
                    uiLink.turnSkipButton.SetActive(false);
                }
            }
        }
        else
        {
            IsDuringPlayerAction = false;
            if (uiLink.Progress == 2)
            {
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Boy, true);
                BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Girl, true);

                if (isPlayerFirst)
                {
                    FSM.ChangeState(BattleState.Monster);
                    uiLink.turnSkipButton.SetActive(false);
                }
                else
                {
                    FSM.ChangeState(BattleState.Settlement);
                    uiLink.turnSkipButton.SetActive(false);
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
}
