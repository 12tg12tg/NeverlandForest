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
    [Header("Actor")]
    public HuntPlayer huntPlayers;
    public Animal animal;

    [Header("Tile")]
    public HuntTilesMaker tileMaker;
    private HuntTile[] tiles;

    [Header("UI")]
    public GameObject failPopUp;
    public GameObject successPopUp;
    public GameObject transparentWindow;
    public TMP_Text huntButtonText;
    public RewardObject reward;
    public Button optionButton;

    [Header("Production")]
    public Production production;

    // 확률
    private int huntPercent;
    private int huntPercentUp;
    private int bushHuntPercent;
    private int totalHuntPercent;

    private bool isHunted = false;

    // 튜토리얼
    private HuntTutorial huntTutorial;

    private void OnEnable()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.Hunting, Hunting);
    }
    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.Hunting, Hunting);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "사냥시작"))
        {
            production.FadeOut();
            tileMaker.InitMakeTiles();
            Init();

            huntPlayers.IsTutorialClear = GameManager.Manager.tm.contentsTutorial.contentsTutorialProceed.Hunt;
            if (!huntPlayers.IsTutorialClear)
            {
                optionButton.interactable = false;
                TutorialInit();
                StartCoroutine(huntTutorial.CoHuntTutorial());
            }
        }
        if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "사냥시작"))
        {
            production.FadeOut();
            tileMaker.InitMakeTiles();
            Init();
            huntPlayers.IsTutorialClear = true;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "재시작"))
        {
            production.FadeOut();
            Init();
            huntPlayers.IsTutorialClear = true;
        }
    }

    public void Init()
    {
        var count = tileMaker.transform.childCount;

        var randomBush = new int[tileMaker.col - 2];
        for (int i = 0; i < randomBush.Length; i++)
        {
            randomBush[i] = Random.Range(0, 3);
        }

        tiles = new HuntTile[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = tileMaker.transform.GetChild(i).GetComponent<HuntTile>();
            tiles[i].bush.gameObject.SetActive(false); // 현재 켜져있는 타일을 다시 꺼주고
            
            var bushIndex = (int)tiles[i].index.y;
            if (bushIndex > 0 && bushIndex < tileMaker.col - 1 &&
                randomBush[bushIndex - 1].Equals((int)tiles[i].index.x))
            {
                tiles[i].bush.gameObject.SetActive(true); // 새롭게 랜덤으로 뽑힌 애들만 켜주도록..
            }
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
        animal.Init();
    }

    private void TutorialInit()
    {
        BottomUIManager.Instance.ButtonInteractive(false);
        huntTutorial = gameObject.AddComponent<HuntTutorial>();
        huntTutorial.huntPlayers = huntPlayers;
        huntTutorial.tile = tiles.Where(x => x.bush.gameObject.activeSelf && (int)x.index.y == 1).Select(x => x).FirstOrDefault();
        huntTutorial.tile.huntingManager = this;
        var huntButton = huntButtonText.gameObject.transform.parent.GetComponent<Button>();
        huntButton.onClick.AddListener(NextTutorialStep);
        huntButton.interactable = false;
        huntTutorial.huntButton = huntButton;
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
        perccent = perccent > 100 ? 100 : perccent;
        huntButtonText.text = $"성공 {perccent}%" + "\n" + "사냥하기";
    }

    private void SetRewardItem()
    {
        // 추후 동물이 얻을 수 있는 아이템 리스트가 생기면 거기에서 가져오게끔 변경 예정
        var tempItemNum = 5;
        var stringId = $"ITEM_{tempItemNum}";
        var item = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(stringId);
        var newItem = new DataAllItem(item);
        newItem.OwnCount = Random.Range(1, 5);

        reward.Item = newItem;
        reward.SetItemSprite(item.IconSprite);
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
        if (isHunted)
        {
            huntPlayers.HuntSuccessAnimation();
            animal.AnimalDead();
            SetRewardItem();

            if (huntTutorial != null)
                huntTutorial.TutorialStep++;
        }
        else
        {
            huntPlayers.HuntFailAnimation();
            animal.AnimalRunAway();
        }
        var resultPopUp = isHunted ? successPopUp : failPopUp;
        animal.AnimalMove(isHunted, () => {
            resultPopUp.SetActive(true);
            transparentWindow.SetActive(true);
        });
        huntPlayers.ReturnBow();
    }

    public void NextScene() => SceneManager.LoadScene("AS_RandomMap");
    public void NextTutorialStep() => huntTutorial.TutorialStep++;
}
