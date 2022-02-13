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

[DefaultExecutionOrder(100)]
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
    public DungeonRewardDiaryManager rewardPopup;
    public GameObject transparentWindow;
    public TMP_Text huntButtonText;
    public Button optionButton;

    [Header("Tutorial")]
    public TutorialManager tm;

    // Ȯ��
    private int huntPercent;
    private int huntPercentUp;
    private int bushHuntPercent;
    private int totalHuntPercent;

    private bool isHunted = false;

    // Ʃ�丮��
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
    private void Start()
    {
        GameManager.Manager.Production.FadeOut();
        tileMaker.InitMakeTiles();
        Init();
        tm.contentsTutorial.Init();
        
        huntPlayers.IsTutorialClear = Vars.UserData.contentsTutorial.Hunt;
        if (!huntPlayers.IsTutorialClear)
        {
            optionButton.interactable = false;
            TutorialInit();
            StartCoroutine(huntTutorial.CoHuntTutorial());
        }
        SoundManager.Instance.Play(SoundType.BG_Hunt);
    }

    #region ��� �Ŵ��� OnGUI -> ���� �Ϸ�Ǹ� �Ⱦ�
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "��ɽ���"))
    //    {
    //        production.FadeOut();
    //        tileMaker.InitMakeTiles();
    //        Init();

    //        huntPlayers.IsTutorialClear = GameManager.Manager.TutoManager.contentsTutorial.contentsTutorialProceed.Hunt;
    //        if (!huntPlayers.IsTutorialClear)
    //        {
    //            optionButton.interactable = false;
    //            TutorialInit();
    //            StartCoroutine(huntTutorial.CoHuntTutorial());
    //        }
    //    }
    //    if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "��ɽ���"))
    //    {
    //        production.FadeOut();
    //        tileMaker.InitMakeTiles();
    //        Init();
    //        huntPlayers.IsTutorialClear = true;
    //    }
    //    if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "�����"))
    //    {
    //        production.FadeOut();
    //        Init();
    //        huntPlayers.IsTutorialClear = true;
    //    }
    //}
    #endregion

    public void Init()
    {
        GameManager.Manager.State = GameState.Hunt;

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
            tiles[i].bush.gameObject.SetActive(false); // ���� �����ִ� Ÿ���� �ٽ� ���ְ�
            
            var bushIndex = (int)tiles[i].index.y;
            if (bushIndex > 0 && bushIndex < tileMaker.col - 1 &&
                randomBush[bushIndex - 1].Equals((int)tiles[i].index.x))
            {
                tiles[i].bush.gameObject.SetActive(true); // ���Ӱ� �������� ���� �ֵ鸸 ���ֵ���..
            }
        }

        huntPlayers.Init();

        // �÷��̾�� ��ġ ���
        huntPlayers.hunter.transform.position =
            tiles.Where(x => x.index.Equals(huntPlayers.CurHunterIndex))
                 .Select(x => x.transform.position).FirstOrDefault();

        huntPlayers.herbalist.transform.position =
            tiles.Where(x => x.index.Equals(huntPlayers.curHerbalistIndex))
                 .Select(x => x.transform.position).FirstOrDefault();

        // ��� �� �߰�Ȯ�� �ʱ�ȭ
        InitHuntPercentage();
        animal.Init();
    }

    private void TutorialInit()
    {
        BottomUIManager.Instance.ButtonInteractive(false);
        huntTutorial = gameObject.AddComponent<HuntTutorial>();
        huntTutorial.tm = tm;
        huntTutorial.Init();
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
        //TODO : ���� + ��/�� = �� ��� �߰��� ���� ����
        var lanternCount = Vars.UserData.uData.LanternCount; // ������ �����ؾ� �ϴ� �κ�
        var step =
            lanternCount < 7 ? 1 :
            lanternCount < 12 ? 2 :
            lanternCount < 16 ? 3 : 4;
        var lanternPercent = step == 1 ? Random.Range(5, 9) : Random.Range(5, 8);

        huntPercent = lanternPercent * step;

        // ��� Ȯ������ �ܰ躰 �� * step
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
        huntButtonText.text = $"���� {perccent}%" + "\n" + "����ϱ�";
    }

    private void SetRewardItem() => rewardPopup.OpenRewardsPopup(GameManager.Manager.reward.GetHuntRewards());

    public void Shooting()
    {
        totalHuntPercent = huntPercent + bushHuntPercent;
        var rnd = Random.Range(0f, 1f);
        var succeeded = isHunted = rnd < totalHuntPercent * 0.01f;
        if (huntTutorial != null) // Ʃ�丮�� ������ �̶��
            succeeded = isHunted = true;
        var pos = animal.transform.position;
        var rndAngle = Random.Range(0, 181);
        var rndAnimalRange = Random.Range(3.6f, 4.6f);
        var failPos = Quaternion.Euler(new Vector3(0f, rndAngle, 0f)) * Vector3.forward * rndAnimalRange;
        pos = succeeded ? pos : pos - failPos;

        Debug.Log($"�� Ȯ��:{totalHuntPercent * 0.01f} > ���� Ȯ��:{rnd}");

        huntPlayers.ShootArrow(pos);
    }

    public void LookOnTarget()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);
        huntPlayers.ShootAnimation(animal.transform.position);
    }
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
        var resultPopUp = isHunted ? rewardPopup.gameObject : failPopUp;
        animal.AnimalMove(isHunted, () => {
            resultPopUp.SetActive(true);
            transparentWindow.SetActive(true);
        });
        huntPlayers.ReturnBow();
    }

    public void NextScene() => GameManager.Manager.LoadScene(GameScene.Dungeon);
    public void NextTutorialStep() => huntTutorial.TutorialStep++;
}
