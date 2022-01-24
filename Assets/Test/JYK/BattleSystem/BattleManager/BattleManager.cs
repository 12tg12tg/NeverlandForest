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
    private MultiTouch multiTouch;

    //Unit
    public List<MonsterUnit> monsters = new List<MonsterUnit>();
    private Queue<MonsterUnit> wave1 = new Queue<MonsterUnit>();
    private Queue<MonsterUnit> wave2 = new Queue<MonsterUnit>();
    private Queue<MonsterUnit> wave3 = new Queue<MonsterUnit>();
    public PlayerBattleController boy;
    public PlayerBattleController girl;
    private PlayerCommand boyInput;
    private PlayerCommand girlInput;

    //Instance
    public Canvas uiCanvas;
    [SerializeField] private BottomUIManager uiManager;
    public TileMaker tileMaker;
    public Image dragSlot;
    public BattleMessage message;
    public BattleMessage cautionMessage;
    public BattleFSM FSM;

    //New Instance
    [SerializeField] private Button battleStartBut;

    //Vars
    private int turn;
    private int curGroup;
    private int totlaWave, curWave, preWaveTurn;
    private const int middleOfStage = 4;
    private bool isDrag;
    private Queue<MonsterCommand> monsterQueue = new Queue<MonsterCommand>();
    private IEnumerable<Tiles> targetTiles;
    private bool isWaitingTileSelect;
    private const float monsterSpeed = 10f;
    public bool isPlayerFirst;
    private int progress;

    //Property
    public int Progress => progress;
    public int Turn { get => turn; set => turn = value; }
    public bool IsWaitingTileSelect { get => isWaitingTileSelect; }
    public Queue<MonsterCommand> MonsterQueue { get => monsterQueue; }
    public bool IsDuringPlayerAction { get; set; }

    private void Awake()
    {
        instance = this;
        boyInput = new PlayerCommand(boy, PlayerType.Boy);
        girlInput = new PlayerCommand(girl, PlayerType.Girl);

    }

    private void Start()
    {
        //Multitouch 활성화
        multiTouch = MultiTouch.Instance;

        //Command 연결
        boy.command = boyInput;
        girl.command = girlInput;
    }


    private void Update()
    {
        if (isDrag)
        {
            dragSlot.transform.position = multiTouch.TouchPos;
        }

        //Debug.Log($"{boyInput.IsUpdated} / {girlInput.IsUpdated}");
    }

    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. Grade & Wave
        //  마지막방전투인지 일반전투인지를 판단하고,
        //  중반이 지나갔는지 아닌지를 판단하고,
        if (isBlueMoonBattle)
        {
            curGroup = 6;
            totlaWave = 3;
        }
        else if (isEndOfDeongun)
        {
            curGroup = Random.Range(4, 6);
            totlaWave = 3;
        }
        else
        {
            totlaWave = Random.Range(2, 4);

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

        // 2. 변수 초기화
        turn = 1;
        preWaveTurn = 1;
        curWave = 0;
        IsDuringPlayerAction = false;
        GameManager.Manager.State = GameState.Battle;

        // 3. 몬스터 (랜덤뽑기) & 배치
        monsters.Clear();
        wave1.Clear();
        wave2.Clear();
        wave3.Clear();

        var monsterElems = DataTableManager.GetTable<MonsterTable>().data.Values;
        var groups = (from n in monsterElems
                      where (n as MonsterTableElem).@group == curGroup
                      select int.Parse(n.id)).ToList();

        void EnqueueMonster(Queue<MonsterUnit> queue, int id) // 지역 메소드
        {
            var tag = (MonsterPoolTag)id;
            var go = MonsterPool.Instance.GetObject(tag);
            var unitSc = go.GetComponent<MonsterUnit>();
            queue.Enqueue(unitSc);
        }

        if (isBlueMoonBattle || isEndOfDeongun)
        {
            if (groups.Count != 3)
                Debug.LogError($"보스전에 몬스터 후보가 {groups.Count}마리");

            // 보스전 - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            var rand = Random.Range(0, groups.Count);
            EnqueueMonster(wave2, groups[rand]);
            EnqueueMonster(wave2, bossIndex);       // 보스 중앙
            rand = Random.Range(0, groups.Count);
            EnqueueMonster(wave2, groups[rand]);

            // Wave1, Wave3
            Queue<MonsterUnit> temp;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    temp = wave1;
                else
                    temp = wave3;

                int monsterInWave = Random.Range(2, 4);
                for (int k = 0; k < monsterInWave; k++)
                {
                    rand = Random.Range(0, groups.Count);
                    EnqueueMonster(temp, groups[rand]);
                }
            }
        }
        else
        {
            // 일반 배틀
            int randWaveCount = Random.Range(2, 4);
            Queue<MonsterUnit> temp = null;
            for (int i = 0; i < curWave; i++)
            {
                if (i == 0)
                    temp = wave1;
                else if (i == 1)
                    temp = wave2;
                else
                    temp = wave3;

                int monsterInWave = Random.Range(2, 4);
                for (int k = 0; k < monsterInWave; k++)
                {
                    var rand = Random.Range(0, groups.Count);
                    EnqueueMonster(temp, groups[rand]);
                }
            }
        }

        SetWavePosition(wave1);
        SetWavePosition(wave2);
        SetWavePosition(wave3);
        //StartWave(1);

        // 플레이어 스탯 전달받기
        // Vars 전역 저장소에서 불러오기.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;
        girl.stats.Hp = (int)Vars.UserData.uData.HerbalistHp;

        //플레이어 배치
        SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //배틀상태 Start
        FSM.ChangeState(BattleState.Start);
    }

    public void WaitUntillSettingDone()
    {
        /* 전투시작 버튼 활성화
           인벤토리에서 트랩류 스킬 테두리 On && 나머지 비활성화 */
        battleStartBut.gameObject.SetActive(true);



    }
    
    public void StartButton() // 버튼용 함수
    {
        battleStartBut.gameObject.SetActive(false);

        var start = FSM.GetState(BattleState.Start) as BattleStart;
        start.IsReadyDone = true;
    }

    //Wave
    public bool IsReadyToNextWave()
    {
        for (int i = 0; i < 3; i++)
        {
            var tile = tileMaker.GetTile(new Vector2(i, 6));
            if (!tile.CanStand)
                return false;
        }
        return true;
    } //마지막 열에 몬스터 한마리씩 설 자리가 있다면 true 반환.

    public void StartWave(int wave)
    {
        Queue<MonsterUnit> temp = null;
        if (wave == 1)
            temp = wave1;
        else if (wave == 2)
            temp = wave2;
        else
            temp = wave3;

        if (temp.Count == 3) // 세마리라면 마지막 몹을 중앙에.
        {
            int i = 0;
            while (temp.Count != 0)
            {
                var monsterSc = temp.Dequeue();
                monsters.Add(monsterSc);
                var tempForCoroutine = monsterSc;
                var tilePos = new Vector2(i, 6);
                tempForCoroutine.ObstacleAdd(tilePos);
                MoveUnitOnTile(tilePos, monsterSc, tempForCoroutine.PlayMoveAnimation, tempForCoroutine.PlayIdleAnimation);
                i++;
            }
        }
        else // 두마리라면 뽑아서 아무데나 안겹치게.
        {
            int randException = Random.Range(0, 3); // 0 || 1 || 2
            int[] indexArr = null;
            if (randException == 0)
                indexArr = new int[] { 1, 2 };
            else if (randException == 1)
                indexArr = new int[] { 0, 2 };
            else
                indexArr = new int[] { 0, 1 };

            int i = 0;
            while (temp.Count != 0)
            {
                var monsterSc = temp.Dequeue();
                monsters.Add(monsterSc);
                var tempForCoroutine = monsterSc;
                var tilePos = new Vector2(indexArr[i], 6);
                tempForCoroutine.ObstacleAdd(tilePos);
                MoveUnitOnTile(tilePos, monsterSc, tempForCoroutine.PlayMoveAnimation, tempForCoroutine.PlayIdleAnimation);
                i++;
            }
        }
    } //매개변수 웨이브를 전투에 입장시키기.

    public void SetWavePosition(Queue<MonsterUnit> waveQueue, bool useCoroutine = false)
    {
        if (waveQueue.Count == 0)
            return;

        int wave;
        if (waveQueue == wave1)
            wave = 1;
        else if (waveQueue == wave2)
            wave = 2;
        else
            wave = 3;

        int count = waveQueue.Count;
        var remainWave = wave - curWave;
        MonsterUnit temp;

        var basePos = tileMaker.GetTile(new Vector2(0, 6)).transform.position;
        var leftPos = tileMaker.GetTile(new Vector2(0, 5)).transform.position;
        var upPos = tileMaker.GetTile(new Vector2(1, 6)).transform.position;
        var spacingX = basePos.x - leftPos.x;
        var spacingZ = upPos.z - basePos.z;

        if (count == 2)
            basePos += new Vector3(0f, 0f, spacingZ / 2);

        for (int i = 0; i < count; i++)
        {
            temp = waveQueue.Dequeue();

            var curPos = temp.transform.position;
            var newPos = basePos + new Vector3(spacingX * remainWave, 0f, i * spacingZ);
            if (!useCoroutine)
                temp.transform.position = newPos;
            else
            {
                var tempForCoroutine = temp;
                temp.PlayMoveAnimation();
                StartCoroutine(Utility.CoTranslate(temp.transform, curPos, newPos, 1f, tempForCoroutine.PlayIdleAnimation));
            }
            waveQueue.Enqueue(temp);
        }
    }

    public void UpdateWave() //조건 확인
    {
        if (curWave == 3)
            return;
        if (!IsReadyToNextWave())
            return;
        if (turn != 1 && turn - preWaveTurn < 1)
            return;
        else
            preWaveTurn = turn;

        curWave++;
        if(curWave == 1)
        {
            StartWave(1);
            SetWavePosition(wave2, true);
            SetWavePosition(wave3, true);
        }
        else if(curWave == 2)
        {
            StartWave(2);
            SetWavePosition(wave3, true);
        }
        else if(curWave == 3)
        {
            StartWave(3);
        }
    }

    //UI
    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
    }

    public void PrintCaution(string message, float time, float fadeDelay, UnityAction action)
    {
        cautionMessage.PrintMessageFadeOut(message, 0.7f, 0.5f, null);
    }

    public void CreateTempSkillUiForDrag(DataPlayerSkill skill)
    {
        dragSlot.gameObject.SetActive(true);
        dragSlot.sprite = skill.SkillTableElem.IconSprite;
        isDrag = true;
    }

    public void CreateTempItemUiForDrag(DataConsumable item)
    {
        dragSlot.gameObject.SetActive(true);
        dragSlot.sprite = item.ItemTableElem.IconSprite;
        isDrag = true;
    }

    public void EndTempUiForDrag()
    {
        dragSlot.gameObject.SetActive(false);
        isDrag = false;
    }

    //Tile
    public void SetUnitOnTile(Vector2 tilePos, UnitBase unit)
    {
        var tile = tileMaker.GetTile(tilePos);

        tile.Units_UnitAdd(unit);
        unit.Pos = tilePos;

        var dest = tile.CenterPos;
        dest.y = unit.transform.position.y;
        unit.transform.position = dest;
    }

    public void MoveUnitOnTile(Vector2 tilePos, MonsterUnit monsterUnit, UnityAction moveStartAction, UnityAction moveEndAction, bool rotateFoward = true)
    {
        var preTile = monsterUnit.CurTile;
        preTile.RemoveUnit(monsterUnit);

        var tile = tileMaker.GetTile(tilePos);

        tile.Units_UnitAdd(monsterUnit);
        monsterUnit.Pos = tilePos;

        MonsterUnit alreadyPlacedMonster = (tile.FrontMonster == monsterUnit) ? tile.BehindMonster : tile.FrontMonster;
        bool isAlreadyBehind = tile.FrontMonster == monsterUnit;

        if (tile.Units_UnitCount() == 2)
        {
            if(isAlreadyBehind)
            {
                // 원래 놓인 몬스터가 뒤쪽이었던 경우.
                //  1) 새 몬스터를 앞포지션으로 이동시킴.

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                StartCoroutine(CoMoveMonster(monsterUnit, frontDest,
                    moveStartAction, moveEndAction, rotateFoward));
            }
            else
            {
                // 원래 놓인 몬스터가 앞쪽이거나 중앙이었던 경우.
                //  1) 새 몬스터를 뒷포지션으로 이동시킴.
                //  2) 기존 몬스터를 앞포지션으로 이동시킴.

                var behindDest = tile.BehindPos;
                behindDest.y = tile.BehindMonster.transform.position.y;

                StartCoroutine(CoMoveMonster(monsterUnit, behindDest,
                    moveStartAction, moveEndAction, rotateFoward));

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                if(frontDest != tile.FrontMonster.transform.position)
                    StartCoroutine(CoMoveMonster(tile.FrontMonster, frontDest,
                        tile.FrontMonster.PlayMoveAnimation, tile.FrontMonster.PlayIdleAnimation));
            }
        }
        else
        {
            var dest = tile.CenterPos;
            dest.y = monsterUnit.transform.position.y;
            StartCoroutine(CoMoveMonster(monsterUnit, dest, moveStartAction, moveEndAction, rotateFoward));
        }
    }

    public IEnumerator CoMoveMonster(MonsterUnit unit, Vector3 dest, UnityAction startAc, UnityAction endAc, bool haveToRotate = true)
    {
        var startRot = Quaternion.LookRotation(unit.transform.forward);
        var destRot = Quaternion.LookRotation(dest - unit.transform.position);

        if (haveToRotate && Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, startRot, destRot, 0.3f));

        yield return new WaitForSeconds(0.3f);

        var speed = (haveToRotate) ? monsterSpeed : monsterSpeed * 2;

        startAc?.Invoke();
        yield return StartCoroutine(Utility.CoTranslate(unit.transform, dest, speed, 0.3f));
        endAc?.Invoke();

        if (haveToRotate && Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, destRot, startRot, 0.3f));
    }

    public void DisplayMonsterTile(SkillRangeType range)
    {
        targetTiles = tileMaker.GetMonsterTiles();
        foreach (var tile in targetTiles)
        {
            tile.HighlightCanAttackSign(range);
        }
    }

    public void UndisplayMonsterTile()
    {
        targetTiles = tileMaker.GetMonsterTiles();
        foreach (var tile in targetTiles)
        {
            tile.ResetHighlightExceptConfirm();
        }
    }

    public void DisplayPlayerTile()
    {
        targetTiles = tileMaker.GetPlayerTiles();
        foreach (var tile in targetTiles)
        {
            tile.HighlightCanConsumeSign();
        }
    }

    public bool IsVaildTargetTile(Tiles tile)
    {
        return targetTiles.Contains(tile);
    }

    public void ReadyTileClick()
    {
        isWaitingTileSelect = true;
    }

    public void EndTileClick()
    {
        isWaitingTileSelect = false;
    }

    //Progress
    public void UpdateProgress()
    {
        progress++;
        BottomUIManager.Instance.UpdateProgress();
    }

    public void ResetProgress()
    {
        progress = 0;
        BottomUIManager.Instance.UpdateProgress();
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

    public void DoCommand(DataConsumable item)
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
            PrintMessage($"승리!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
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
        var monster = monsters.Where(x => x.obstacles != null)
                              .Select(x => x)
                              .ToList();

        for (int i = 0; i < monster.Count; i++)
        {
            monster[i].ObstacleHit();
        }

        action?.Invoke();
    }
    private void OnGUI()
    {
        if (GUILayout.Button("블루문X, 마지막전투X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(false);
        }
        if (GUILayout.Button("블루문X, 마지막전투O", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(false, true);
        }
        if (GUILayout.Button("블루문O, 마지막전투X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(true);
        }

        if(GUILayout.Button(" ← ", GUILayout.Width(200f), GUILayout.Height(200f)))
        {
            var monster0 = monsters[0];
            var tile = monster0.CurTile;
            Tiles foward = tileMaker.GetTile(new Vector2(tile.index.x, tile.index.y - 1));

            MoveUnitOnTile(foward.index, monster0, null, null);
        }
        if (GUILayout.Button(" → ", GUILayout.Width(200f), GUILayout.Height(200f)))
        {
            var monster0 = monsters[0];
            var tile = monster0.CurTile;
            Tiles foward = tileMaker.GetTile(new Vector2(tile.index.x, tile.index.y + 1));

            MoveUnitOnTile(foward.index, monster0, null, null);
        }

        if (GUILayout.Button("Lantern 2", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.ConsumeLantern(12);
        }
        if (GUILayout.Button("Lantern 3", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.FullingLantern(12);
        }

        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "올가미"))
        {
            uiManager.curObstacleType = ObstacleType.Lasso;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "부비트랩"))
        {
            if(!Inventory_Virtual.instance.isLasso)
                uiManager.curObstacleType = ObstacleType.BoobyTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "나무트랩"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.WoodenTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 300, 100, 100), "가시트랩"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.ThornTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 400, 100, 100), "장애물"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.Barrier;
        }


        //if (GUILayout.Button("테스트 셔플", GUILayout.Width(200f), GUILayout.Height(100f)))
        //{
        //    List<int> list = new List<int>(new int[] { 0, 1, 2, 3, 4, 5 });
        //    Utility.Shuffle(list);
        //    string str = "";
        //    foreach (var item in list)
        //    {
        //        str += item + ", ";
        //    }
        //    Debug.Log(str);
        //}
        //if (GUILayout.Button("Move Monster"))
        //{
        //    StartCoroutine(BattleManager.Instance.MoveUnitOnTile(new Vector2(1, 0), monster[0]));
        //}
        //if(GUILayout.Button("Boy Clicked"))
        //{
        //    OpenSkillUI(PlayerType.Boy);
        //}
        //if (GUILayout.Button("Girl Clicked"))
        //{
        //    OpenSkillUI(PlayerType.Girl);
        //}
        //if(GUILayout.Button("Who First"))
        //{
        //    Debug.Log($"Boy : {boyInput.IsFirst} / {boyInput.IsSecond}");
        //    Debug.Log($"Girl : {girlInput.IsFirst} / {girlInput.IsSecond}");
        //}
        //if(GUILayout.Button("Move 1 Foward"))
        //{
        //    var monster0 = monster[0];
        //    var tile = monster0.CurTile;
        //    Tiles foward;
        //    if(tile.TryGetFowardTile(out foward, 1))
        //    {
        //        PlaceUnitOnTile(foward.index, monster0, true);
        //    }
        //}


    }
}
