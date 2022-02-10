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
public enum BattleInitState { None, Dungeon, Tutorial, Bluemoon }

[DefaultExecutionOrder(10)]
public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance { get => instance; }

    //Battle Static
    public static BattleInitState initState;

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
    public BattleTutorial tutorial;

    //Instance
    [Header("몬스터 관련 UI 캔버스 연결")]
    public Canvas uiCanvas;

    [Header("배틀 FSM 연결")]
    public BattleFSM FSM;

    //ObjectPool Parents
    [Header("오브젝트풀 부모 오브젝트")]
    public Transform monsterParent;
    public Transform uiParent;
    public Transform damageUiParent;
    public Transform trapParent;
    public Transform projectileParent;

    //Vars
    public bool isTutorial;
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

        //시작
        switch (initState)
        {
            case BattleInitState.None:
                Init(false, false);
                break;
            case BattleInitState.Dungeon:
                Init(false, true);
                break;            
            case BattleInitState.Tutorial:
                tutorial.StartDutorial();
                break;
            case BattleInitState.Bluemoon:
                Init(true);
                break;
        }

        GameManager.Manager.Production?.FadeOut();
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
        boy.stats.Hp = (int)Vars.UserData.uData.Hp;

        //플레이어 배치
        tileLink.SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        tileLink.SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //배틀상태 Start
        FSM.ChangeState(BattleState.Start);
    }

    public void TutorialInit(bool reInit = false)
    {
        VarInit();
        isTutorial = true;
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

        if (reInit)
        {
            waveLink.totalWave = 2;

            var mushbae = FindMonsterToId(0);
            var mushbro = FindMonsterToId(1);
            monsters.Add(mushbae);
            monsters.Add(mushbro);

            var tile = TileMaker.Instance.GetTile(new Vector2(1, 6));
            tile.Units_UnitAdd(mushbae);
            mushbae.Pos = tile.index;
            tile.Units_UnitAdd(mushbro);
            mushbro.Pos = tile.index;

            var fointDest = tile.FrontPos;
            fointDest.y = tile.BehindMonster.transform.position.y;
            mushbae.transform.position = fointDest;

            var behindDest = tile.BehindPos;
            behindDest.y = tile.BehindMonster.transform.position.y;
            mushbro.transform.position = behindDest;

            mushbae.SetActionCommand();
            mushbro.SetActionCommand();

            //플레이어 배치
            boy.PlayIdleAnimation();
            girl.PlayIdleAnimation();

            // 인벤토리 비우기
            DataAllItem temp2;
            var tempInventory2 = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
            foreach (var item in tempInventory2)
            {
                temp2 = new DataAllItem(item);
                Vars.UserData.RemoveItemData(temp2);
            }

            // 화살
            temp2 = new DataAllItem(costLink.arrowElem);
            temp2.OwnCount = 19;
            Vars.UserData.AddItemData(temp2); // 20발

            // 오일
            temp2 = new DataAllItem(costLink.oilElem);
            temp2.OwnCount = 3;
            Vars.UserData.AddItemData(temp2); // 4개

            // 포션
            temp2 = new DataAllItem(costLink.potionElem);
            temp2.OwnCount = 1;
            Vars.UserData.AddItemData(temp2); // 1개

            // 랜턴밝기
            ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
            ConsumeManager.FullingLantern(customBattle.lanternCount); // 풀

            //배틀상태 Start
            FSM.ChangeState(BattleState.Player);
            uiLink.UpdateProgress(BattleUI.ProgressIcon.Girl);
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            waveLink.wave1.Add(null);
            waveLink.wave2.Add(null);
            waveLink.wave3.Add(null);
        }

        waveLink.totalWave = 2;

        waveLink.wave1[1] = FindMonsterToId(0);
        waveLink.wave1[1].Pos = new Vector2(1, 6);
        waveLink.wave1[1].SetActionCommand();
        waveLink.wave2[1] = FindMonsterToId(1);
        waveLink.wave2[1].Pos = new Vector2(1, 6);
        waveLink.wave2[1].SetActionCommand();



        // 인벤토리 비우기
        DataAllItem temp;
        var tempInventory = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
        foreach (var item in tempInventory)
        {
            temp = new DataAllItem(item);
            Vars.UserData.RemoveItemData(temp);
        }

        // 화살
        temp = new DataAllItem(costLink.arrowElem);
        temp.OwnCount = 20;
        Vars.UserData.AddItemData(temp); // 20발

        // 오일
        temp = new DataAllItem(costLink.oilElem);
        temp.OwnCount = 4;
        Vars.UserData.AddItemData(temp); // 4개

        // 포션
        temp = new DataAllItem(costLink.potionElem);
        temp.OwnCount = 1;
        Vars.UserData.AddItemData(temp); // 1개

        // 나무트랩
        temp = new DataAllItem(costLink.woodenTrapElem);
        temp.OwnCount = 1;
        Vars.UserData.AddItemData(temp); // 1개


        // 랜턴밝기
        ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
        ConsumeManager.FullingLantern(customBattle.lanternCount); // 풀

        // 웨이브 시작
        waveLink.SetWavePosition(waveLink.wave1);
        waveLink.SetWavePosition(waveLink.wave2);
        waveLink.SetWavePosition(waveLink.wave3);

        // 플레이어 스탯 전달받기
        // Vars 전역 저장소에서 불러오기.
        boy.stats.Hp = (int)Vars.UserData.uData.Hp;

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

        // Hp
        Vars.maxHp = customBattle.hp;
        ConsumeManager.CostDataReset();
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
        go.transform.SetParent(monsterParent);
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

    public void DoCommand(PlayerType type, Vector2 target, DataPlayerSkill skill, bool isDrag)
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
        attacker.TurnInit(ActionType.Skill, isDrag);
    }

    public void EndOfPlayerAction()
    {
        // 이겼는지 체크
        var monsterlist = monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if(monsterlist.Count == 0)
        {
            if (waveLink.IsAllWaveClear())
            {
                uiLink.turnSkipTrans.SetActive(false);
                uiLink.progressTrans.SetActive(false);
                uiLink.PrintMessage($"승리!", 2.5f, () =>
                    {
                        // ★승리
                        if (isTutorial) // 튜토리얼
                        {
                            tutorial.isWin = true;
                            /*보상창 띄우기??*/
                        }
                        else // 평상시
                        {
                            /*보상창 띄우기*/
                            boy.PlayWinAnimation();
                            girl.PlayWinAnimation();
                            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
                            //SceneManager.LoadScene("AS_RandomMap");
                        }
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
                    uiLink.turnSkipTrans.SetActive(false);
                }
                else
                {
                    FSM.ChangeState(BattleState.Settlement);
                    uiLink.turnSkipTrans.SetActive(false);
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
                    if (!isTutorial || !tutorial.lockAutoBattleStateChange)
                    {
                        FSM.ChangeState(BattleState.Monster);
                        uiLink.turnSkipTrans.SetActive(false);
                    }
                }
                else
                {
                    FSM.ChangeState(BattleState.Settlement);
                    uiLink.turnSkipTrans.SetActive(false);
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
