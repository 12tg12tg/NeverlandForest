using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public enum HuntingEvent
{
    PlayerMove,
    AnimalEscapePercentUp,
    AnimalEscape,
    Hunting,
}
public class HuntingManager : MonoBehaviour
{
    public HuntPlayer huntPlayers;
    public HuntTilesMaker tileMaker;
    public Animal animal;
    public GameObject result;
    public WorldMapMaker worldMap;

    // UI
    public Image getItemImage;
    public TMP_Text huntButtonText;
    public TMP_Text popupText;

    private HuntTile[] tiles;

    // 확률
    private int huntPercent;
    private int huntPercentUp;
    private int bushHuntPercent;
    private int totalHuntPercent;

    private bool isHunted = false;
    
    [Header("튜토리얼")]
    private HuntTutorial huntTutorial;
    public RectTransform target;
    public Button disableUI;

    private void OnEnable()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.Hunting, Hunting);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "사냥시작"))
        {
            tileMaker.InitMakeTiles();
            Init();
        }
        if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "튜토리얼"))
        {
            tileMaker.InitMakeTiles();
            Init();

            var isTutorialProceed = GameManager.Manager.tm.contentsTutorial.contentsTutorialProceed.Hunt;
            if (!isTutorialProceed)
            {
                TutorialInit();
                StartCoroutine(huntTutorial.CoHuntTutorial());
            }
        }
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "다음"))
        {
            huntTutorial.TutorialStep++;
        }
    }

    public void Init()
    {
        // 이 부분은 최초 실행 때만 하는 것인지 사냥이 시작될 때 마다 해야하는 것인지 모르겠음
        var count = tileMaker.transform.childCount;
        tiles = new HuntTile[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = tileMaker.transform.GetChild(i).GetComponent<HuntTile>();
        }

        huntPlayers.Init();

        // 플레이어들 위치 잡기
        huntPlayers.hunter.transform.position =
            tiles.Where(x => x.index.Equals(huntPlayers.CurHunterIndex))
                 .Select(x => x.transform.position).FirstOrDefault();

        huntPlayers.herbalist.transform.position =
            tiles.Where(x => x.index.Equals(huntPlayers.curHerbalistIndex))
                 .Select(x => x.transform.position).FirstOrDefault();

        // 사냥 및 발각확률 초기화
        InitHuntPercentage();
        animal.InitEscapingPercentage();
    }

    private void TutorialInit()
    {
        huntTutorial = gameObject.AddComponent<HuntTutorial>();
        huntTutorial.huntPlayers = huntPlayers;
        huntTutorial.tile = tiles.Where(x => x.bush != null && (int)x.index.y == 1).Select(x => x).FirstOrDefault();
        huntTutorial.tile.huntingManager = this;
        target.GetComponent<Button>().onClick.AddListener(TutorialSetActiveFalse);
        var huntButton = huntButtonText.gameObject.transform.parent.GetComponent<Button>();
        huntButton.onClick.AddListener(NextTutorialStep);
        huntButton.interactable = false;
        huntTutorial.huntButton = huntButton;
        disableUI.interactable = false;
        huntTutorial.target = target;
    }

    private void InitHuntPercentage()
    {
        //TODO : 랜턴 + 낮/밤 = 빛 기능 추가시 변경 예정
        var lanternCount = Vars.UserData.uData.LanternCount; // 빛으로 변경해야 하는 부분
        var step =
            lanternCount < 7 ? 1 :
            lanternCount < 12 ? 2 :
            lanternCount < 16 ? 3 : 4;
        var lanternPercent = step == 1 ? Random.Range(5, 9) : Random.Range(5, 8);

        huntPercent = lanternPercent * step;

        // 사냥 확률업은 단계별 값 * step
        huntPercentUp =
            (step == 1 ? 14 :
            step == 2 ? 7 :
            step == 3 ? 5 : 4) * step;

        HuntPercentagePrint(huntPercentUp);
    }

    

    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.Hunting, Hunting);
    }

    private void OnBush(object[] vals)
    {
        bushHuntPercent = (bool)vals[1] && vals.Length.Equals(2) ? 5 : 0;
    }

    private void HuntPercentageUp(object[] vals)
    {
        huntPercent = (bool)vals[0] && vals.Length.Equals(2) ? huntPercent + huntPercentUp : huntPercent;
        totalHuntPercent = huntPercent + bushHuntPercent;
        HuntPercentagePrint(totalHuntPercent);
    }
    private void HuntPercentagePrint(int perccent)
    {
        huntButtonText.text = "사냥" + "\n" + $"성공 {perccent}%";
        popupText.text = $"현재 확률 : {perccent}%" + "\n" + "사냥하시겠습니까";
    }

    private void GetItem()
    {
        // 추후 동물이 얻을 수 있는 아이템 리스트가 생기면 거기에서 가져오게끔 변경 예정
        var tempItemNum = 5;
        var stringId = $"ITEM_{tempItemNum}";
        var item = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(stringId);
        var newItem = new DataAllItem(item);
        newItem.OwnCount = Random.Range(1, 5);
        getItemImage.sprite = item.IconSprite;
        Vars.UserData.AddItemData(newItem);
    }

    public void Shooting()
    {
        totalHuntPercent = huntPercent + bushHuntPercent;
        var rnd = Random.Range(0f, 1f);
        var succeeded = isHunted = rnd < totalHuntPercent * 0.01f;
        if (huntTutorial != null) // 튜토리얼 진행중 이라면
            succeeded = isHunted = true;
        var pos = animal.transform.position;
        var rndAngle = Random.Range(0, 181);
        var rndAnimalRange = Random.Range(3.6f, 4.6f);
        var failPos = Quaternion.Euler(new Vector3(0f, rndAngle, 0f)) * Vector3.forward * rndAnimalRange;
        pos = succeeded ? pos : pos - failPos;

        Debug.Log($"내 확률:{totalHuntPercent * 0.01f} > 랜덤 확률:{rnd}");

        huntPlayers.ShootArrow(pos);
    }

    public void LookOnTarget() => huntPlayers.ShootAnimation(animal.transform.position);
    private void Hunting(object[] vals)
    {
        var textTMP = result.GetComponentInChildren<TMP_Text>();
        if (isHunted)
        {
            huntPlayers.HuntSuccessAnimation();
            animal.AnimalDead();
            getItemImage.gameObject.SetActive(true);
            textTMP.text = "Hunting Success";
            GetItem();

            if (huntTutorial != null) // 튜토리얼 진행중 이라면
            {
                huntTutorial.TutorialStep++;
                Debug.Log(huntTutorial.TutorialStep);
            }
        }
        else
        {
            huntPlayers.HuntFailAnimation();
            animal.AnimalRunAway();
            textTMP.text = "Hunting Fail";
        }
        huntPlayers.ReturnBow();
        animal.AnimalMove(isHunted, () => result.SetActive(true));
    }

    public void NextScene() => SceneManager.LoadScene("AS_RandomMap");

    public void NextTutorialStep()
    {
        huntTutorial.TutorialStep++;
        Debug.Log(huntTutorial.TutorialStep);
    }
    public void TutorialSetActiveFalse() => huntTutorial.SetActive(false);
}
