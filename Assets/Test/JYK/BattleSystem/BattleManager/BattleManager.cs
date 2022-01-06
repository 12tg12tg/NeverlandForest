using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NewTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BattleCommand { }
public class MonsterCommand : BattleCommand
{
    public MonsterUnit attacker;
    public Vector2 target;
    public MonsterActionType actionType;
    public int Ordering { get; set; }

    public MonsterCommand(MonsterUnit mUnit)
    {
        attacker = mUnit;
        Ordering = mUnit.Speed;
    }

}
public class PlayerCommand : BattleCommand
{
    private bool isUpdate;
    public PlayerBattleController attacker;
    public Vector2 target;
    public DataPlayerSkill skill;
    public DataConsumable item;
    public ActionType actionType;
    public PlayerType type;
    public PlayerCommand(PlayerBattleController pUnit, PlayerType type)
    {
        attacker = pUnit;
        this.type = type;
    }
    public bool IsUpdated { get => isUpdate; }
    public bool IsFirst { get; set; }
    public bool IsSecond { get => isUpdate ? !IsFirst : false; }
    public void Clear()
    {
        isUpdate = false;
        target = Vector2.zero;
        skill = null;
        item = null;
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
    public void Create(Vector2 target, DataConsumable item)
    {
        if (isUpdate)
            Clear();
        this.target = target;
        this.item = item;
        isUpdate = true;
        actionType = ActionType.Item;
    }
}

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
    public GameObject playerTurnUI;
    public UserInputPanel userInputPanel;

    //Vars
    private bool isDrag;
    private Queue<PlayerCommand> comandQueue = new Queue<PlayerCommand>();
    private Queue<MonsterCommand> monsterQueue = new Queue<MonsterCommand>();
    private IEnumerable<Tiles> targetTiles;
    private bool isWaitingTileSelect;

    //Property
    public SkillButton CurClickedButton { get; set; }
    public bool IsWaitingTileSelect { get => isWaitingTileSelect; }
    public Queue<PlayerCommand> CommandQueue { get => comandQueue; }
    public Queue<MonsterCommand> MonsterQueue { get => monsterQueue; }

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

        //스킬목록 전달받기
        Init();

        //플레이어 배치
        PlaceUnitOnTile(new Vector2(0, 0), girl.Stats);
        PlaceUnitOnTile(new Vector2(1, 0), boy.Stats);

        //몬스터 (랜덤뽑기) & 배치
        PlaceUnitOnTile(new Vector2(0, 6), monster[0]);
        PlaceUnitOnTile(new Vector2(2, 6), monster[1]);
        PlaceUnitOnTile(new Vector2(1, 6), monster[2]);

        //스킬창 Init
        hunterUI.Init(PlayerType.Boy, this, Vars.BoySkillList, herbologistUI);
        herbologistUI.Init(PlayerType.Girl, this, Vars.GirlSkillList, hunterUI);
    }


    private void Update()
    {
        if(isDrag)
        {
            dragSlot.transform.position = multiTouch.TouchPos;
        }

        Debug.Log($"{boyInput.IsUpdated} / {girlInput.IsUpdated}");
    }

    //초기화
    public void Init()
    {
        /*플레이어 스킬 목록 전달받기*/
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

        //몬스터 리스트 전달받기

        //플레이어 스탯 전달받기
            // Vars 전역 저장소에서 불러오기.
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

    public void PlaceUnitOnTile(Vector2 tilePos, UnitBase unit, UnityAction action, bool isAnimation = false)
    {
        var tile = tileMaker.GetTile(tilePos);
        if (tile.CanStand)
        {
            tile.units.Add(unit);
            unit.Pos = tilePos;

            var dest = tile.WolrdPos;
            dest.y = unit.transform.position.y;

            if (isAnimation)
            {
                StartCoroutine(
                    Utility.CoTranslate(unit.transform, unit.transform.position, dest, 0.7f, action)) ;
            }
            else
            {
                unit.transform.position = dest;
            }
        }
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
        userInputPanel.Clear();
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataPlayerSkill skill)
    {
        PlayerCommand command;
        PlayerCommand another;
        PlayerBattleController attacker;
        if (type == PlayerType.Boy)
        {
            command = boyInput;
            another = girlInput;

            attacker = boy;
        }
        else
        {
            command = girlInput;
            another = boyInput;

            attacker = girl;
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

        if(!another.IsUpdated)
        {
            userInputPanel.Init(command, null);
        }
        else
        {
            userInputPanel.Init(another, command);
        }
    }

    public void UpdateComand(PlayerType type, Vector2 target, DataConsumable item)
    {
        PlayerCommand command;
        PlayerCommand another;
        PlayerBattleController attacker;
        if (type == PlayerType.Boy)
        {
            command = boyInput;
            another = girlInput;

            attacker = boy;
        }
        else
        {
            command = girlInput;
            another = boyInput;

            attacker = girl;
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

        if (!another.IsUpdated)
        {
            userInputPanel.Init(command, null);
        }
        else
        {
            userInputPanel.Init(another, command);
        }
    }

    public void FinishTurn()
    {
        if (!boyInput.IsUpdated || !girlInput.IsUpdated)
        {
            string str = string.Empty;
            if (!boyInput.IsUpdated && !girlInput.IsUpdated)
                str = "사냥꾼과 약초학자의 행동이 정해지지 않았습니다.";
            else if (boyInput.IsUpdated && !girlInput.IsUpdated)
                str = "약초학자의 행동이 정해지지 않았습니다.";
            else if (!boyInput.IsUpdated && girlInput.IsUpdated)
                str = "사냥꾼의 행동이 정해지지 않았습니다.";

            PrintCaution(str, 0.7f, 0.5f, null);
            return;
        }
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
            hunterUI.Close();
            herbologistUI.Close();
        }
    }

    public void CommandArrangeSwap()
    {
        if(boyInput.IsFirst)
        {
            boyInput.IsFirst = false;
            girlInput.IsFirst = true;
        }
        else
        {
            boyInput.IsFirst = true;
            girlInput.IsFirst = false;
        }
    }



    private void OnGUI()
    {
        if(GUILayout.Button("Boy Clicked"))
        {
            OpenSkillUI(PlayerType.Boy);
        }
        if (GUILayout.Button("Girl Clicked"))
        {
            OpenSkillUI(PlayerType.Girl);
        }
        if(GUILayout.Button("Who First"))
        {
            Debug.Log($"Boy : {boyInput.IsFirst} / {boyInput.IsSecond}");
            Debug.Log($"Girl : {girlInput.IsFirst} / {girlInput.IsSecond}");
        }
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
