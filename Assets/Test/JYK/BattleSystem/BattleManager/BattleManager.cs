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
public enum BattleInitState { None, Dungeon, Tutorial, Bluemoon, BluemoonSet }

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
    public BluemoonSetting blueMoonSetLink;

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
    public bool isBluemoonSet;
    private const int middleOfStage = 4;
    private Queue<MonsterCommand> monsterActionQueue = new Queue<MonsterCommand>();

    [Header("현재 턴 이전 턴 확인")]
    public int turn;
    public int preWaveTurn;

    [Header("현재 몬스터들 정보 확인")]
    [SerializeField] private int curGroup;
    public int curMonstersNum;

    [Header("선공 / 후공 확인")]
    public bool isPlayerFirst;

    [Header("몬스터 그룹 설정")]
    public CustomBattle customBattle;

    [Header("패배 확인")]
    public bool isLose;

    //Property
    public int Turn { get => turn; set => turn = value; }
    public bool IsWaitingTileSelect { get => tileLink.isWaitingTileSelect; }
    public Queue<MonsterCommand> MonsterActionQueue { get => monsterActionQueue; }
    public bool IsDuringPlayerAction { get; set; }

    private readonly int lanternCount = 18;

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
            case BattleInitState.BluemoonSet:
                InitBlueMoonSet();
                break;
        }
        //InitBlueMoonSet();

        GameManager.Manager.Production?.FadeOut();
        SoundManager.Instance?.Play(SoundType.BG_Battle);
    }

    // Release
    public void Release()
    {
        // 오브젝트풀 반환
        var monsters = new List<MonsterUnit>(waveLink.AliveMonsters);
        foreach (var monster in monsters)
        {
            monster.Release(); // 몬스터 -> 몬스터UI -> 토큰 전부 반환
        }

        // 트랩류
        var tiles = TileMaker.Instance.TileList;
        var traps = from n in tiles
                    where n.obstacle != null
                    select n.obstacle;
        foreach (var trap in traps)
        {
            trap.Release();
        }
    }

    // 초기화
    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. 변수 초기화
        VarInit();

        if(isBlueMoonBattle)
        {
            blueMoonSetLink.LoadTrapData();
        }

        if (!customBattle.useCustomMode)
        {
            // 2. Grade & Wave
            //  마지막방전투인지 일반전투인지를 판단하고,
            //  중반이 지나갔는지 아닌지를 판단하고,
            SetGradeAndWave(isBlueMoonBattle, isEndOfDeongun);

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
        // 1. 변수 초기화
        VarInit();
        isTutorial = true;
        monsters.Clear();
        waveLink.wave1.Clear();
        waveLink.wave2.Clear();
        waveLink.wave3.Clear();

        curMonstersNum = 2;

        for (int i = 0; i < 3; i++)
        {
            waveLink.wave1.Add(null);
            waveLink.wave2.Add(null);
            waveLink.wave3.Add(null);
        }

        if (reInit)
        {
            turn = 2;
            preWaveTurn = 1;
            waveLink.curWave = 3;
            IsDuringPlayerAction = false;
            GameManager.Manager.State = GameState.Battle;

            waveLink.wave1.Clear();
            waveLink.wave2.Clear();
            waveLink.wave3.Clear();

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

            // 인벤토리 비우기
            DataAllItem temp2;
            var tempInventory2 = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
            foreach (var item in tempInventory2)
            {
                temp2 = new DataAllItem(item);
                Vars.UserData.RemoveItemData(temp2);
            }

            // 화살
            temp2 = new DataAllItem(costLink.ArrowElem);
            temp2.OwnCount = 19;
            Vars.UserData.AddItemData(temp2); // 19발

            // 오일
            temp2 = new DataAllItem(costLink.OilElem);
            temp2.OwnCount = 3;
            Vars.UserData.AddItemData(temp2); // 4개

            //// 포션
            //temp2 = new DataAllItem(costLink.PotionElem);
            //temp2.OwnCount = 1;
            //Vars.UserData.AddItemData(temp2); // 1개

            // 나무도막
            var allItemTable2 = DataTableManager.GetTable<AllItemDataTable>();
            temp2 = new DataAllItem(allItemTable2.GetData<AllItemTableElem>("ITEM_1"));
            temp2.OwnCount = 6;
            Vars.UserData.AddItemData(temp2); // 3개

            // 삽
            allItemTable2 = DataTableManager.GetTable<AllItemDataTable>();
            temp2 = new DataAllItem(allItemTable2.GetData<AllItemTableElem>("ITEM_13"));
            temp2.OwnCount = 1;
            Vars.UserData.AddItemData(temp2); // 3개

            // 도끼
            allItemTable2 = DataTableManager.GetTable<AllItemDataTable>();
            temp2 = new DataAllItem(allItemTable2.GetData<AllItemTableElem>("ITEM_12"));
            temp2.OwnCount = 1;
            Vars.UserData.AddItemData(temp2); // 1개

            BottomUIManager.Instance.UpdateCostInfo();

            // 랜턴밝기
            ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
            ConsumeManager.FullingLantern(lanternCount); // 풀

            //배틀상태 Start
            FSM.ChangeState(BattleState.Player);
            uiLink.UpdateProgress(BattleUI.ProgressIcon.Girl);
            BottomUIManager.Instance.InteractiveSkillButton(PlayerType.Girl, false);
            BottomUIManager.Instance.UpdateSkillInteractive();
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
        temp = new DataAllItem(costLink.ArrowElem);
        temp.OwnCount = 20;
        Vars.UserData.AddItemData(temp); // 20발

        // 오일
        temp = new DataAllItem(costLink.OilElem);
        temp.OwnCount = 4;
        Vars.UserData.AddItemData(temp); // 4개

        //// 포션
        //temp = new DataAllItem(costLink.PotionElem);
        //temp.OwnCount = 1;
        //Vars.UserData.AddItemData(temp); // 1개

        // 나무트랩
        temp = new DataAllItem(costLink.WoodenTrapElem);
        temp.OwnCount = 1;
        Vars.UserData.AddItemData(temp); // 1개

        // 나무도막
        var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_1"));
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp); // 3개

        // 삽
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_13"));
        temp.OwnCount = 1;
        Vars.UserData.AddItemData(temp); // 1개

        BottomUIManager.Instance.UpdateCostInfo();

        // 랜턴밝기
        ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
        ConsumeManager.FullingLantern(lanternCount); // 풀

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
        int totalMonsterNum = 0;
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
                    ++totalMonsterNum;
                    realWave[k] = FindMonsterToId((int)customWave[k]);
                    realWave[k].Pos = new Vector2(k, 6);
                    realWave[k].SetActionCommand();
                }
            }
        }

        curMonstersNum = totalMonsterNum;

        // 인벤토리 비우기
        DataAllItem temp2;
        var tempInventory2 = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
        foreach (var item in tempInventory2)
        {
            temp2 = new DataAllItem(item);
            Vars.UserData.RemoveItemData(temp2);
        }

        // 화살
        DataAllItem temp;
        int total = 0;
        var inventory = Vars.UserData.HaveAllItemList;
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.ArrowElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.ArrowElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.arrowNum;
        Vars.UserData.AddItemData(temp);

        // 쇠화살
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.IronArrowElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.IronArrowElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.ironArrowNum;
        Vars.UserData.AddItemData(temp);

        // 오일
        foreach (var item in inventory)
        {
            if (item.itemId == costLink.OilElem.id)
            {
                total += item.OwnCount;
            }
        }
        temp = new DataAllItem(costLink.OilElem);
        temp.OwnCount = total;
        Vars.UserData.RemoveItemData(temp);
        temp.OwnCount = customBattle.oilNum;
        Vars.UserData.AddItemData(temp);

        BottomUIManager.Instance.UpdateCostInfo();

        // 트랩 류
        temp = new DataAllItem(costLink.WoodenTrapElem);
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp);

        temp = new DataAllItem(DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_17"));
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp);

        temp = new DataAllItem(DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_18"));
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp);

        temp = new DataAllItem(DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_15"));
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp);

        temp = new DataAllItem(DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>("ITEM_14"));
        temp.OwnCount = 3;
        Vars.UserData.AddItemData(temp);


        // 랜턴밝기
        ConsumeManager.ConsumeLantern((int)Vars.UserData.uData.LanternCount);
        ConsumeManager.FullingLantern(customBattle.lanternCount);

        // Hp
        Vars.maxHp = customBattle.hp;
        ConsumeManager.CostDataReset();
    }
    private void SetGradeAndWave(bool isBlueMoonBattle, bool isEndOfDeongun = false)
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

            float curCol;
            if (Vars.UserData.WorldMapPlayerData != null)
                curCol = Vars.UserData.WorldMapPlayerData.currentIndex.y + 1; //1 ~ 9
            else
                curCol = 7;
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

        if (isBlueMoonBattle)
        {
            // 블루문은 총 7마리
            int totalMonsterNum = 7;
            curMonstersNum = totalMonsterNum;

            // 보스전 - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            waveLink.wave2[1] = FindMonsterToId(bossIndex);       // 보스 중앙
            waveLink.wave2[1].Pos = new Vector2(1, 6);
            waveLink.wave2[1].SetActionCommand();

            int randNum = Random.Range(0, 3); // 보스 제외 몬스터 수
            if(randNum == 0)
            {
            }
            else if(randNum == 1)
            {
                int rand = Random.Range(0, 2);
                if(rand == 0)
                {
                    var randId = Random.Range(0, groups.Count);
                    waveLink.wave2[0] = FindMonsterToId(groups[randId]);
                    waveLink.wave2[0].Pos = new Vector2(0, 6);
                    waveLink.wave2[0].SetActionCommand();
                }
                else
                {
                    var randId = Random.Range(0, groups.Count);
                    waveLink.wave2[2] = FindMonsterToId(groups[randId]);
                    waveLink.wave2[2].Pos = new Vector2(2, 6);
                    waveLink.wave2[2].SetActionCommand();
                }
            }
            else // 2
            {
                var randId = Random.Range(0, groups.Count);
                waveLink.wave2[0] = FindMonsterToId(groups[randId]);
                waveLink.wave2[0].Pos = new Vector2(0, 6);
                waveLink.wave2[0].SetActionCommand();
                randId = Random.Range(0, groups.Count);
                waveLink.wave2[2] = FindMonsterToId(groups[randId]);
                waveLink.wave2[2].Pos = new Vector2(2, 6);
                waveLink.wave2[2].SetActionCommand();
            }

            // Wave1, Wave3
            totalMonsterNum -= randNum + 1; // 4 ~ 6
            randNum = Random.Range(1, 4); // 1 ~ 3

            while (totalMonsterNum - randNum > 3)
                ++randNum;

            MakeNormalWave(groups, waveLink.wave1, randNum);
            MakeNormalWave(groups, waveLink.wave3, totalMonsterNum - randNum);
        }
        else if (isEndOfDeongun)
        {
            // 보스방은 총 4~5마리
            int totalMonsterNum = Random.Range(4, 6);
            curMonstersNum = totalMonsterNum;

            // 보스전 - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            waveLink.wave2[1] = FindMonsterToId(bossIndex);       // 보스 중앙
            waveLink.wave2[1].Pos = new Vector2(1, 6);
            waveLink.wave2[1].SetActionCommand();

            int randNum = Random.Range(0, 3); // 보스 제외 몬스터 수
            if (randNum == 0)
            {
            }
            else if (randNum == 1)
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    var randId = Random.Range(0, groups.Count);
                    waveLink.wave2[0] = FindMonsterToId(groups[randId]);
                    waveLink.wave2[0].Pos = new Vector2(0, 6);
                    waveLink.wave2[0].SetActionCommand();
                }
                else
                {
                    var randId = Random.Range(0, groups.Count);
                    waveLink.wave2[2] = FindMonsterToId(groups[randId]);
                    waveLink.wave2[2].Pos = new Vector2(2, 6);
                    waveLink.wave2[2].SetActionCommand();
                }
            }
            else // 2
            {
                var randId = Random.Range(0, groups.Count);
                waveLink.wave2[0] = FindMonsterToId(groups[randId]);
                waveLink.wave2[0].Pos = new Vector2(0, 6);
                waveLink.wave2[0].SetActionCommand();
                randId = Random.Range(0, groups.Count);
                waveLink.wave2[2] = FindMonsterToId(groups[randId]);
                waveLink.wave2[2].Pos = new Vector2(2, 6);
                waveLink.wave2[2].SetActionCommand();
            }

            Debug.Log($"2웨이브 : {randNum + 1}");

            // Wave1, Wave3
            totalMonsterNum -= randNum + 1; // 1 ~ 4
            int max = (totalMonsterNum > 3) ? 3 : totalMonsterNum;
            randNum = Random.Range(1, max + 1);

            MakeNormalWave(groups, waveLink.wave1, randNum);
            MakeNormalWave(groups, waveLink.wave3, totalMonsterNum - randNum);
            Debug.Log($"1웨이브 : {randNum}");
            Debug.Log($"3웨이브 : {totalMonsterNum - randNum}");
        }
        else
        {
            int totalMonsterNum = 4;
            curMonstersNum = totalMonsterNum;

            // 일반 배틀
            List<MonsterUnit> temp = null;
            if(waveLink.totalWave == 3)
            {
                int exceptNum = Random.Range(1, 4); // 1 2 3 
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        temp = waveLink.wave1;
                    else if (i == 1)
                        temp = waveLink.wave2;
                    else
                        temp = waveLink.wave3;

                    if (i == exceptNum)
                        MakeNormalWave(groups, temp, 2);
                    else
                        MakeNormalWave(groups, temp, 1);
                }
            }
            else if (waveLink.totalWave == 2)
            {
                var rand = Random.Range(1, 4);
                MakeNormalWave(groups, waveLink.wave1, rand);
                totalMonsterNum -= rand;
                MakeNormalWave(groups, waveLink.wave2, totalMonsterNum);
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
    public void MakeNormalWave(List<int> idGroup, List<MonsterUnit> wave, int createNum)
    {
        int exceptCol = -1;
        if (createNum != 3)
            exceptCol = Random.Range(0, 3);

        int rand;
        int colIndex = 0;
        for (int k = 0; k < createNum; k++, colIndex++)
        {
            rand = Random.Range(0, idGroup.Count);
            if ((createNum == 2 && colIndex == exceptCol)
                || createNum == 1 && colIndex != exceptCol)
            {
                colIndex++;
            }
            wave[colIndex] = FindMonsterToId(idGroup[rand]);
            wave[colIndex].Pos = new Vector2(colIndex, 6);
            wave[colIndex].SetActionCommand();
        }
    }
    public void InitBlueMoonSet()
    {
        // 게임 상태 설정
        VarInit();
        FSM.ChangeState(BattleState.Start);
        tileLink.SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        tileLink.SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        // 트랩정보 불러오기
        blueMoonSetLink.LoadTrapData();

        // ui 설정
        uiLink.backToCampButton.gameObject.SetActive(true);
        uiLink.backToCampButton.interactable = true;

        //Vars.UserData.UserItemInit(); // 임시 ★★★★★★★★★★

        // 바텀 ui
        BottomUIManager.Instance.ItemButtonInit();

        // 변수설정
        isBluemoonSet = true;

        // 시작
        uiLink.PrintMessage("블루문 습격 대비!", 2.5f, () => inputLink.WaitUntillSettingDone());
    }

    // Command
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
                        Win();
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

    // Settlement
    public void Lose()
    {
        isLose = true;
        GameManager.Manager.GameOver(GameOverType.BattleLoss);
        //Release();
    }
    public void Win()
    {
        // ★승리
        if (isTutorial) // 튜토리얼
        {
            tutorial.isWin = true;
            boy.PlayWinAnimation();
            girl.PlayWinAnimation();
            directingLink.LandDownLantern();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.item);
            //uiLink.OpenRewardPopup();
        }
        else // 평상시
        {
            uiLink.OpenRewardPopup();
            boy.PlayWinAnimation();
            girl.PlayWinAnimation();
            directingLink.LandDownLantern();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.item);
        }

        Release();
    }
}
