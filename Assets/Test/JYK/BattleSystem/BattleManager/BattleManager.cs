using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using NewTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerComand
{
    private bool isUpdate;
    public PlayerBattleUnit attacker;
    public Vector2 target;
    public DataPlayerSkill skill;
    public DataCunsumable item;
    public ActionType actionType;
    public PlayerComand(PlayerBattleUnit stats)
    {
        attacker = stats;
    }
    public bool IsUpdated { get => isUpdate; }
    public bool IsFirst { get; set; }
    public bool IsSecond { get => isUpdate ? !IsFirst : false; }
    public void Clear()
    {
        isUpdate = false;
        attacker = null;
        target = Vector2.zero;
        skill = null;
    }
    public void Create(Vector2 target, DataPlayerSkill skill)
    {
        if (isUpdate)
            Clear();
        this.target = target;
        this.skill = skill;
        isUpdate = true;
        actionType = ActionType.Skill;
    }
    public void Create(Vector2 target, DataCunsumable item)
    {
        if (isUpdate)
            Clear();
        this.target = target;
        this.item = item;
        isUpdate = true;
        actionType = ActionType.Item;
    }
}

public enum PlayerType { Boy, Girl }
public enum ActionType { Skill, Item }


public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance { get => instance; }
    private MultiTouch multiTouch;

    //Unit
    public List<TestMonster> monster = new List<TestMonster>();
    public PlayerStats boy;
    public PlayerStats girl;
    private PlayerComand boyInput;
    private PlayerComand girlInput;

    // 추가
    public PlayerBattleUnit boyChar;
    public PlayerBattleUnit girlChar;

    //Canvas
    public CanvasScaler cs;

    //Instance
    public TileMaker tileMaker;
    public Image dragSlot;
    public BattleSkillInfo info;
    public SkillSelectUI hunterUI;
    public SkillSelectUI herbologistUI;
    public BattleMessage message;
    public BattleFSM FSM;
    public GameObject playerTurnUI;

    //Vars
    private bool isDrag;
    private Queue<PlayerComand> comandQueue = new Queue<PlayerComand>();

    private Queue<TestMonster> monsterQueue = new Queue<TestMonster>();

    public Queue<PlayerComand> CommandQueue
    {
        get => comandQueue;
    }

    public Queue<TestMonster> MonsterQueue
    {
        get => monsterQueue;
    }

    private void Awake()
    {       
        instance = this;
        
    }

    private void Start()
    {
        boyInput = new PlayerComand(boyChar);
        girlInput = new PlayerComand(girlChar);

        //Multitouch 활성화
        multiTouch = MultiTouch.Instance;

        //스킬목록 전달받기
        Init();

        //플레이어 배치
        PlaceUnitOnTile(new Vector2(0, 0), girl);
        PlaceUnitOnTile(new Vector2(1, 0), boy);

        //몬스터 (랜덤뽑기) & 배치
        PlaceUnitOnTile(new Vector2(0, 6), monster[0]);
        PlaceUnitOnTile(new Vector2(2, 6), monster[1]);

        //스킬창 Init
        hunterUI.Init(PlayerType.Boy, this, Vars.UserData.boySkillList, herbologistUI);
        herbologistUI.Init(PlayerType.Girl, this, Vars.UserData.girlSkillList, hunterUI);
    }


    private void Update()
    {
        if(isDrag)
        {
            dragSlot.transform.position = multiTouch.TouchPos;
        }

        //Debug.Log($"{boyInput.IsUpdated} / {girlInput.IsUpdated}");
    }

    //초기화
    public void Init()
    {
        /*플레이어 스킬 목록 전달받기*/
        var boy = Vars.UserData.boySkillList;
        var girl = Vars.UserData.girlSkillList;

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

        //몬스터 리스트 전달받기

        //플레이어 스탯 전달받기
            // Vars 전역 저장소에서 불러오기.
    }    

    //UI
    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
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

    public void CreateTempItemUiForDrag(DataCunsumable item)
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
    
    public void OpenSkillInfo(DataPlayerSkill skill, Vector2 pos)
    {
        info.gameObject.SetActive(true);
        info.Init(skill, pos);
    }

    public void OpenItemInfo(DataCunsumable item, Vector2 pos)
    {
        info.gameObject.SetActive(true);
        info.Init(item, pos);
    }

    //Tile
    public void PlaceUnitOnTile(Vector2 tilePos, UnitBase unit, bool isAnimation = false)
    {
        var tile = tileMaker.GetTile(tilePos);
        if(tile.CanStand)
        {
            tile.units.Add(unit);
            unit.Pos = tilePos;

            var dest = tile.WolrdPos;
            dest.y = unit.transform.position.y;

            if (isAnimation)
            {
                StartCoroutine(
                    Utility.CoTranslate(unit.transform, unit.transform.position, dest, 0.7f));
            }
            else
            {
                unit.transform.position = dest;
            }
        }  
    }

    public void DisplayMonsterTile()
    {
        var list = tileMaker.GetMonsterTiles();
        foreach (var tile in list)
        {
            tile.HighlightCanAttackSign();
        }
    }

    public void DisplayPlayerTile()
    {
        var list = tileMaker.GetPlayerTiles();
        foreach (var tile in list)
        {
            tile.HighlightCanConsumeSign();
        }
    }

    public void DisplayTileClear()
    {
        tileMaker.SetAllTileClear();
    }

    //Command
    public void ClearCommand()
    {
        boyInput.Clear();
        girlInput.Clear();
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataPlayerSkill skill)
    {
        PlayerComand command;
        PlayerComand another;
        //PlayerBattleUnit attacker;
        if (type == PlayerType.Boy)
        {
            command = boyInput;
            another = girlInput;

            //attacker = boyChar;
        }
        else
        {
            command = girlInput;
            another = boyInput;

            //attacker = girlChar;
        }

        if (!command.IsUpdated && !another.IsUpdated)
        {
            command.IsFirst = true;
            another.IsFirst = false;
        }
        else if (command.IsUpdated && another.IsUpdated)
        {
            another.IsFirst = true;
            command.IsFirst = false;
        }

        command.Create(target, skill);
        Debug.Log("d");
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataCunsumable item)
    {
        PlayerComand command;
        PlayerComand another;
        //PlayerBattleUnit attacker;
        if (type == PlayerType.Boy)
        {
            command = boyInput;
            another = girlInput;

            //attacker = boyChar;
        }
        else
        {
            command = girlInput;
            another = boyInput;

            //attacker = girlChar;
        }

        if (!command.IsUpdated && !another.IsUpdated)
        {
            command.IsFirst = true;
            another.IsFirst = false;
        }
        else if (command.IsUpdated && another.IsUpdated)
        {
            another.IsFirst = true;
            command.IsFirst = false;
        }

        command.Create(target, item);
    }

    public void FinishTurn()
    {
        if (!boyInput.IsUpdated && !girlInput.IsUpdated)
            Debug.Log("사냥꾼과 약초학자의 행동이 정해지지 않았습니다. 정말 턴을 마치시겠습니까?");
        else if (boyInput.IsUpdated && !girlInput.IsUpdated)
            Debug.Log("약초학자의 행동이 정해지지 않았습니다. 정말 턴을 마치시겠습니까?");
        else if (!boyInput.IsUpdated && girlInput.IsUpdated)
            Debug.Log("사냥꾼의 행동이 정해지지 않았습니다. 정말 턴을 마치시겠습니까?");
        else
        {
            if (boyInput.IsFirst)
            {
                comandQueue.Enqueue(boyInput);
                comandQueue.Enqueue(girlInput);
            }
            else
            {
                comandQueue.Enqueue(girlInput);
                comandQueue.Enqueue(boyInput);
            }
            FSM.ChangeState(BattleState.Action);
        }
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Boy Clicked"))
        {
            OpenSkillUI(PlayerType.Boy);
            boyInput.attacker = boyChar;
        }
        if (GUILayout.Button("Girl Clicked"))
        {
            OpenSkillUI(PlayerType.Girl);
            girlInput.attacker = girlChar;
        }
        if(GUILayout.Button("Who First"))
        {
            Debug.Log($"Boy : {boyInput.IsFirst} / {boyInput.IsSecond}");
            Debug.Log($"Girl : {girlInput.IsFirst} / {girlInput.IsSecond}");
        }
        if(GUILayout.Button("Move 1 Foward"))
        {
            var monster0 = monster[0];
            var tile = monster0.CurTile;
            Tiles foward;
            if(tile.TryGetFowardTile(out foward, 1))
            {
                PlaceUnitOnTile(foward.index, monster0, true);
            }
        }
    }
}
