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
    private List<MonsterUnit> wave1 = new List<MonsterUnit>();
    private List<MonsterUnit> wave2 = new List<MonsterUnit>();
    private List<MonsterUnit> wave3 = new List<MonsterUnit>();
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
        //Multitouch Ȱ��ȭ
        multiTouch = MultiTouch.Instance;

        //Command ����
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

    // �ʱ�ȭ
    public void GradeWaveInit(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
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
    }
    public void VarInit()
    {
        turn = 1;
        preWaveTurn = 1;
        curWave = 0;
        IsDuringPlayerAction = false;
        GameManager.Manager.State = GameState.Battle;
    }
    public void MonsterInit(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        monsters.Clear();
        wave1.Clear();
        wave2.Clear();
        wave3.Clear();
        for (int i = 0; i < 3; i++)
        {
            wave1.Add(null);
            wave2.Add(null);
            wave3.Add(null);
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
            wave2[0] = FindMonsterToId(groups[rand]);

            wave2[1] = FindMonsterToId(bossIndex);       // ���� �߾�

            rand = Random.Range(0, groups.Count);
            wave2[2] = FindMonsterToId(groups[rand]);

            for (int i = 0; i < 3; i++)
            {
                wave2[i].Pos = new Vector2(i, 6);
                wave2[i].SetActionCommand();
            }

            // Wave1, Wave3
            List<MonsterUnit> temp;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    temp = wave1;
                else
                    temp = wave3;

                MakeNormalWave(groups, temp);
            }
        }
        else
        {
            // �Ϲ� ��Ʋ
            int randWaveCount = Random.Range(2, 4);
            List<MonsterUnit> temp = null;
            for (int i = 0; i < curWave; i++)
            {
                if (i == 0)
                    temp = wave1;
                else if (i == 1)
                    temp = wave2;
                else
                    temp = wave3;

                MakeNormalWave(groups, temp);
            }
        }
    }
    MonsterUnit FindMonsterToId(int monsterId) // ���� �޼ҵ�
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
        //  ���������������� �Ϲ����������� �Ǵ��ϰ�,
        //  �߹��� ���������� �ƴ����� �Ǵ��ϰ�,
        GradeWaveInit(isBlueMoonBattle, isEndOfDeongun);

        // 2. ���� �ʱ�ȭ
        VarInit();

        // 3. ���� (�����̱�) & ��ġ
        MonsterInit(isBlueMoonBattle, isEndOfDeongun);

        SetWavePosition(wave1);
        SetWavePosition(wave2);
        SetWavePosition(wave3);

        // �÷��̾� ���� ���޹ޱ�
        // Vars ���� ����ҿ��� �ҷ�����.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;
        girl.stats.Hp = (int)Vars.UserData.uData.HerbalistHp;

        //�÷��̾� ��ġ
        SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //��Ʋ���� Start
        FSM.ChangeState(BattleState.Start);
    }

    // �÷��̾� �Է� ���
    public void WaitUntillSettingDone()
    {
        /* �������� ��ư Ȱ��ȭ
           �κ��丮���� Ʈ���� ��ų �׵θ� On && ������ ��Ȱ��ȭ */
        battleStartBut.gameObject.SetActive(true);
    }   
    public void StartButton() // ��ư�� �Լ�
    {
        battleStartBut.gameObject.SetActive(false);

        var start = FSM.GetState(BattleState.Start) as BattleStart;
        start.IsReadyDone = true;
    }

    //Wave
    public bool IsAllWaveClear()
    {
        return wave1.Count == 0 && wave2.Count == 0 && wave3.Count == 0;
    }
    public bool IsReadyToNextWave()
    {
        for (int i = 0; i < 3; i++)
        {
            var tile = tileMaker.GetTile(new Vector2(i, 6));
            if (!tile.CanStand)
                return false;
        }
        return true;
    } //������ ���� ���� �Ѹ����� �� �ڸ��� �ִٸ� true ��ȯ.

    public void StartWave(int wave)
    {
        List<MonsterUnit> temp = null;
        if (wave == 1)
            temp = wave1;
        else if (wave == 2)
            temp = wave2;
        else
            temp = wave3;

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] == null)
                continue;
            monsters.Add(temp[i]);
            var tempForCoroutine = temp[i];
            var tilePos = new Vector2(i, 6);
            tempForCoroutine.ObstacleAdd(tilePos);
            MoveUnitOnTile(tilePos, tempForCoroutine, tempForCoroutine.PlayMoveAnimation, tempForCoroutine.PlayIdleAnimation);
        }

    } //�Ű����� ���̺긦 ������ �����Ű��.

    public void SetWavePosition(List<MonsterUnit> waveList, bool useCoroutine = false)
    {
        if (waveList.Count == 0)
            return;

        int wave;
        if (waveList == wave1)
            wave = 1;
        else if (waveList == wave2)
            wave = 2;
        else
            wave = 3;

        int count = waveList.Count;
        var remainWave = wave - curWave;
        MonsterUnit temp;

        var basePos = tileMaker.GetTile(new Vector2(0, 6)).transform.position;
        var leftPos = tileMaker.GetTile(new Vector2(0, 5)).transform.position;
        var upPos = tileMaker.GetTile(new Vector2(1, 6)).transform.position;
        var spacingX = basePos.x - leftPos.x;
        var spacingZ = upPos.z - basePos.z;

        //if (count == 2)
        //    basePos += new Vector3(0f, 0f, spacingZ / 2);

        for (int i = 0; i < count; i++)
        {
            if (waveList[i] == null)
                continue;

            var curPos = waveList[i].transform.position;
            var newPos = basePos + new Vector3(spacingX * remainWave, 0f, i * spacingZ);
            if (!useCoroutine)
                waveList[i].transform.position = newPos;
            else
            {
                var tempForCoroutine = waveList[i];
                waveList[i].PlayMoveAnimation();
                StartCoroutine(Utility.CoTranslate(waveList[i].transform, curPos, newPos, 1f, tempForCoroutine.PlayIdleAnimation));
            }
        }
    }

    public void UpdateWave() //���� Ȯ��
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
                // ���� ���� ���Ͱ� �����̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.

                var frontDest = tile.FrontPos;
                frontDest.y = tile.FrontMonster.transform.position.y;

                StartCoroutine(CoMoveMonster(monsterUnit, frontDest,
                    moveStartAction, moveEndAction, rotateFoward));
            }
            else
            {
                // ���� ���� ���Ͱ� �����̰ų� �߾��̾��� ���.
                //  1) �� ���͸� ������������ �̵���Ŵ.
                //  2) ���� ���͸� ������������ �̵���Ŵ.

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
        // �̰���� üũ
        var monsterlist = monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if(monsterlist.Count == 0)
        {
            PrintMessage($"�¸�!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
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
        if (GUILayout.Button("��繮X, ����������X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(false);
        }
        if (GUILayout.Button("��繮X, ����������O", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(false, true);
        }
        if (GUILayout.Button("��繮O, ����������X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            Init(true);
        }

        if(GUILayout.Button(" �� ", GUILayout.Width(200f), GUILayout.Height(200f)))
        {
            var monster0 = monsters[0];
            var tile = monster0.CurTile;
            Tiles foward = tileMaker.GetTile(new Vector2(tile.index.x, tile.index.y - 1));

            MoveUnitOnTile(foward.index, monster0, null, null);
        }
        if (GUILayout.Button(" �� ", GUILayout.Width(200f), GUILayout.Height(200f)))
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

        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "�ð���"))
        {
            uiManager.curObstacleType = ObstacleType.Lasso;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "�κ�Ʈ��"))
        {
            if(!Inventory_Virtual.instance.isLasso)
                uiManager.curObstacleType = ObstacleType.BoobyTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "����Ʈ��"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.WoodenTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 300, 100, 100), "����Ʈ��"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.ThornTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 400, 100, 100), "��ֹ�"))
        {
            if(!Inventory_Virtual.instance.isLasso)
            uiManager.curObstacleType = ObstacleType.Barrier;
        }



        //if (GUILayout.Button("�׽�Ʈ ����", GUILayout.Width(200f), GUILayout.Height(100f)))
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
