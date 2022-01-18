using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum MonsterActionType { None, Attack, Move }
public enum PlayerType { None, Boy, Girl }
public enum ActionType { Skill, Item }


public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance { get => instance; }
    private MultiTouch multiTouch;

    //Unit
    public List<MonsterUnit> monster = new List<MonsterUnit>();
    private Queue<MonsterUnit> wave1 = new Queue<MonsterUnit>();
    private Queue<MonsterUnit> wave2 = new Queue<MonsterUnit>();
    private Queue<MonsterUnit> wave3 = new Queue<MonsterUnit>();
    public PlayerBattleController boy;
    public PlayerBattleController girl;
    private PlayerCommand boyInput;
    private PlayerCommand girlInput;

    //Canvas
    public CanvasScaler cs;

    //Instance
    public TileMaker tileMaker;
    public Image dragSlot;
    public BattleSkillInfo info;
    public SkillSelectUI hunterUI;
    public SkillSelectUI herbologistUI;
    public BattleMessage message;
    public BattleMessage cautionMessage;
    public BattleFSM FSM;
    public SkillSelectUI trapUI;

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

    //Property
    public int Turn { get => turn; set => turn = value; }
    public SkillButton CurClickedButton { get; set; }
    public bool IsWaitingTileSelect { get => isWaitingTileSelect; }
    public Queue<MonsterCommand> MonsterQueue { get => monsterQueue; }
    public bool IsDuringPlayerAction { get; set; }

    private void Awake()
    {
        instance = this;
        boyInput = new PlayerCommand(boy, PlayerType.Boy);
        girlInput = new PlayerCommand(girl, PlayerType.Girl);

        //��ų��� ���޹ޱ�
        TempInit();
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

    //�ʱ�ȭ
    public void TempInit()
    {
        /*�ӽ� - �÷��̾� ��ų ��� ���޹ޱ�*/
        var boy = Vars.BoySkillList;
        var girl = Vars.GirlSkillList;

        var skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("0");
        var data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("1");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("2");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("3");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("4");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("5");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("6");
        data = new DataPlayerSkill(skill);
        girl.Add(data);
    }

    public void Init(bool isBlueMoonBattle, bool isEndOfDeongun = false)
    {
        // 1. Grade & Wave
        //  ���������������� �Ϲ����������� �Ǵ��ϰ�,
        //  �߹��� ���������� �ƴ����� �Ǵ��ϰ�,
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

        // 2. ���� �ʱ�ȭ
        turn = 1;
        preWaveTurn = 1;
        curWave = 0;

        // 3. ���� (�����̱�) & ��ġ
        monster.Clear();
        wave1.Clear();
        wave2.Clear();
        wave3.Clear();

        var monsterElems = DataTableManager.GetTable<MonsterTable>().data.Values;
        var groups = (from n in monsterElems
                      where (n as MonsterTableElem).@group == curGroup
                      select int.Parse(n.id)).ToList();

        void EnqueueMonster(Queue<MonsterUnit> queue, int id) // ���� �޼ҵ�
        {
            var tag = (MonsterPoolTag)id;
            var go = MonsterPool.Instance.GetObject(tag);
            var unitSc = go.GetComponent<MonsterUnit>();
            queue.Enqueue(unitSc);
        }

        if (isBlueMoonBattle || isEndOfDeongun)
        {
            if (groups.Count != 3)
                Debug.LogError($"�������� ���� �ĺ��� {groups.Count}����");

            // ������ - Wave2
            var bossIndex = groups.Max();
            groups.Remove(bossIndex);

            var rand = Random.Range(0, groups.Count);
            EnqueueMonster(wave2, groups[rand]);
            EnqueueMonster(wave2, bossIndex);       // ���� �߾�
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
            // �Ϲ� ��Ʋ
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

        // �÷��̾� ���� ���޹ޱ�
        // Vars ���� ����ҿ��� �ҷ�����.
        boy.stats.Hp = (int)Vars.UserData.uData.HunterHp;
        girl.stats.Hp = (int)Vars.UserData.uData.HerbalistHp;

        //�÷��̾� ��ġ
        SetUnitOnTile(new Vector2(0, 0), girl.Stats);
        SetUnitOnTile(new Vector2(1, 0), boy.Stats);

        //��ųâ Init
        hunterUI.Init(PlayerType.Boy, this, Vars.BoySkillList, herbologistUI);
        herbologistUI.Init(PlayerType.Girl, this, Vars.GirlSkillList, hunterUI);
        trapUI.Init(PlayerType.None, this, Vars.GirlSkillList, null);

        //��Ʋ���� Start
        FSM.ChangeState(BattleState.Start);
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
    } //������ ���� ���� �Ѹ����� �� �ڸ��� �ִٸ� true ��ȯ.

    public void StartWave(int wave)
    {
        Queue<MonsterUnit> temp = null;
        if (wave == 1)
            temp = wave1;
        else if (wave == 2)
            temp = wave2;
        else
            temp = wave3;

        if (temp.Count == 3) // ��������� ������ ���� �߾ӿ�.
        {
            int i = 0;
            while (temp.Count != 0)
            {
                var monsterSc = temp.Dequeue();
                monster.Add(monsterSc);
                var tempForCoroutine = monsterSc;
                MoveUnitOnTile(new Vector2(i, 6), monsterSc, tempForCoroutine.PlayMoveAnimation, tempForCoroutine.PlayIdleAnimation);
                i++;
            }
        }
        else // �θ������ �̾Ƽ� �ƹ����� �Ȱ�ġ��.
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
                monster.Add(monsterSc);
                var tempForCoroutine = monsterSc;
                MoveUnitOnTile(new Vector2(indexArr[i], 6), monsterSc, tempForCoroutine.PlayMoveAnimation, tempForCoroutine.PlayIdleAnimation);
                i++;
            }
        }
    } //�Ű����� ���̺긦 ������ �����Ű��.

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

    public void UpdateWave() //���� Ȯ��
    {
        if (curWave == 3)
            return;
        if (!IsReadyToNextWave())
            return;
        if (turn != 1 && turn - preWaveTurn < 2)
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

    //Battle Ready
    public void ActivateTrapSetUI()
    {
        trapUI.gameObject.SetActive(true);
    }

    public void TrapSettingDone()
    {
        var startState = FSM.GetState(BattleState.Start) as BattleStart;
        startState.IsTrapDone = true;
        trapUI.Close();
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

    public void OpenSkillUI(PlayerType type)
    {
        if (type == PlayerType.Boy)
            hunterUI.Open();
        else
            herbologistUI.Open();
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

    public void OpenSkillInfo(SkillButton clickedButton, DataPlayerSkill skill, Vector2 pos)
    {
        CurClickedButton = clickedButton;
        info.gameObject.SetActive(true);
        info.Init(skill, pos);
    }

    public void OpenItemInfo(SkillButton clickedButton, DataConsumable item, Vector2 pos)
    {
        CurClickedButton = clickedButton;
        info.gameObject.SetActive(true);
        info.Init(item, pos);
    }

    //Tile
    public void SetUnitOnTile(Vector2 tilePos, UnitBase unit)
    {
        var tile = tileMaker.GetTile(tilePos);

        tile.units.Add(unit);
        unit.Pos = tilePos;

        var dest = tile.CenterPos;
        dest.y = unit.transform.position.y;
        unit.transform.position = dest;
    }

    public void MoveUnitOnTile(Vector2 tilePos, MonsterUnit monsterUnit, UnityAction moveStartAction, UnityAction moveEndAction)
    {
        var preTile = monsterUnit.CurTile;
        preTile.RemoveUnit(monsterUnit);

        var tile = tileMaker.GetTile(tilePos);

        tile.units.Add(monsterUnit);
        monsterUnit.Pos = tilePos;

        if(tile.units.Count == 2)
        {
            var frontDest = tile.FrontPos;
            frontDest.y = tile.FrontMonster.transform.position.y;

            var frontMonster = tile.FrontMonster;

            StartCoroutine(CoMoveMonster(frontMonster, frontDest,
                frontMonster.PlayMoveAnimation, frontMonster.PlayIdleAnimation));

            var dest = tile.BehindPos;
            dest.y = monsterUnit.transform.position.y;
            StartCoroutine(CoMoveMonster(monsterUnit, dest, moveStartAction, moveEndAction));
        }
        else
        {
            var dest = tile.CenterPos;
            dest.y = monsterUnit.transform.position.y;
            StartCoroutine(CoMoveMonster(monsterUnit, dest, moveStartAction, moveEndAction));
        }
    }

    public IEnumerator CoMoveMonster(MonsterUnit unit, Vector3 dest, UnityAction startAc, UnityAction endAc)
    {
        var startRot = Quaternion.LookRotation(unit.transform.forward);
        var destRot = Quaternion.LookRotation(dest - unit.transform.position);
        if (Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, startRot, destRot, 0.3f));

        yield return new WaitForSeconds(0.3f);

        startAc?.Invoke();
        yield return StartCoroutine(Utility.CoTranslate(unit.transform, dest, monsterSpeed, 0.3f));
        endAc?.Invoke();

        if (Quaternion.Angle(startRot, destRot) > 0f)
            yield return StartCoroutine(Utility.CoRotate(unit.transform, destRot, startRot, 0.3f));
    }

    public void DisplayMonsterTile()
    {
        targetTiles = tileMaker.GetMonsterTiles();
        foreach (var tile in targetTiles)
        {
            tile.HighlightCanAttackSign();
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

    //Command
    public void ClearCommand()
    {
        boyInput.Clear();
        girlInput.Clear();
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataPlayerSkill skill)
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
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataConsumable item)
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

        command.Create(target, item);
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
        if (GUILayout.Button("Wave Update", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            UpdateWave();
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
