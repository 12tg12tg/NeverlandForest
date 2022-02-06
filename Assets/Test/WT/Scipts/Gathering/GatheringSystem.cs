using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum GatheringType
{
    MainDunguen,
    Path
}
public class GatheringSystem : MonoBehaviour
{
   
    [Header("플레이어")]
    private Animator playerAnimation;
    private Animator playerAnimationBoy;
    public PlayerDungeonUnit womenplayer;
    public PlayerDungeonUnit boyPlayer;
    [Header("다이어리,재확인")]
    public DungeonRewardDiaryManager dungeonrewarddiaryManager;
    public ReconfirmPanelManager reconfirmPanelManager;
    [Header("채집판넬관련")]
    public GameObject gatheringPanel;
    [Header("채집텍스트관련")]
    public TextMeshProUGUI gatheringtext;
    public TextMeshProUGUI gatheringLanternLeveltext;
    public TextMeshProUGUI gatheringToolConsumetext;
    public TextMeshProUGUI gatheringToolCompleteTimeText;
    public TextMeshProUGUI toolName;
    public TextMeshProUGUI toolCount;
    public TextMeshProUGUI gatheringHandConsumeText;
    public TextMeshProUGUI handCompleteTimeText;

    //New
    [Header("채집이미지관련")]
    public Image toolImage;
    public Image handimage;
    [Header("채집아이콘관련")]
    public GameObject toolitemicon;
    public GameObject handitemicon;
    [Header("채집시간관련")]
    public GameObject toolconsumeTime;
    public GameObject handconsumeTime;
    public GameObject toolcompleteTime;
    public GameObject handcompleteTime;
    public GameObject toolremainTime;
    public GameObject handremainTime;
    [Header("채집버튼관련")]
    public GameObject toolbutton;
    public GameObject handbutton;


    [Header("채집보상아이템관련")]
    public GameObject gatheringInDungeonRewardObject;

    private List<GameObject> gatherings = new List<GameObject>();
    private List<GatheringInDungeonRewardObject> gatheringRewardList = 
        new List<GatheringInDungeonRewardObject>();
    private List<DataAllItem> rewardList = new List<DataAllItem>();
    public GameObject gatheringParent;
    public List<DataAllItem>  selecteditemList = new List<DataAllItem>();
    private static GatheringSystem instance;
    public static GatheringSystem Instance => instance;
    private GameObject reward;
    private GameObject subreward;
    private bool isMove;
   
   
    public GatheringType curgatheringType = GatheringType.MainDunguen;

    private Coroutine coWomenMove;
    public Coroutine CoWomenMove => coWomenMove;

    public GatheringObject curSelectedObj;
    private float speed = 3f;
    private Vector3 manbeforePosition;
    private Vector3 womenbeforePosition;
    int count = 0;
    private List<Vector3> posCheckList = new List<Vector3>();
    public bool PositionCheck(Vector3 pos)
    {
        if (posCheckList.Count > 1)
        {
            for (int i = 0; i < posCheckList.Count; i++)
            {
                if (Mathf.Abs(posCheckList[i].z - pos.z) < 0.3f)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void Start()
    {
        instance = this;
        playerAnimation = womenplayer.GetComponent<Animator>();
        playerAnimationBoy = boyPlayer.GetComponent<Animator>();
    }
    public void DeleteObj()
    {
        for (int i = 0; i < gatherings.Count; i++)
        {
            Destroy(gatherings[i]);
        }
        gatherings.Clear();
    }
    public void YesIGathering()
    {
        ToolPopUp();
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        boyPlayer.IsCoMove = true;
        if (coWomenMove == null)
        {
            PlayWalkAnimationBoy();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
    }
    private void PopUp()
    {
        gatheringPanel.SetActive(true);
        gatheringtext.gameObject.SetActive(true);
        gatheringtext.text = " 채집을 시작하겠습니까";
        isMove = true;
    }
    private void ToolPopUp()
    {
        dungeonrewarddiaryManager.gameObject.SetActive(true);
        dungeonrewarddiaryManager.OpenGatheringInDungeon();
        if (ConsumeManager.CurTimeState ==TimeState.DayTime)
        {
            gatheringLanternLeveltext.text = "랜턴" + Vars.UserData.uData.lanternState.ToString();
            if (ConsumeManager.CurLanternState ==LanternState.Level3)
            {
                gatheringLanternLeveltext.text += "(30분보정중)";
            }
            else if (ConsumeManager.CurLanternState == LanternState.Level4)
            {
                gatheringLanternLeveltext.text += "(1시간보정중)";
            }
            else
            {
                gatheringLanternLeveltext.text += "(밝기가낮아 보정받지못합니다)";
            }
        }
        var lanternstate = ConsumeManager.CurLanternState;
        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                TreeGatheing(lanternstate);
                break;
            case GatheringObjectType.Pit:
                PitGatheing(lanternstate);
                break;
            case GatheringObjectType.Herbs:
                HerbsGatheing(lanternstate);
                break;
            case GatheringObjectType.Mushroom:
                MushroomGatheing(lanternstate);
                break;
            default:
                break;
        }
    }

    private void TreeGatheing(LanternState lanternstate)
    {

        toolconsumeTime.SetActive(true);
        handconsumeTime.SetActive(true);

        toolitemicon.SetActive(true);
        handitemicon.SetActive(true);

        toolcompleteTime.SetActive(true);
        handcompleteTime.SetActive(true);

        toolbutton.SetActive(true);
        handbutton.SetActive(true);

        toolremainTime.SetActive(true);
        handremainTime.SetActive(true);

        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
           gatheringToolConsumetext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "시 " + "\n"
             + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "시 " + "\n"
           + (Vars.UserData.uData.CurIngameMinute ).ToString() + "분";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "시간은 1시간을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "시 " + "\n"
            + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
        }
        else
        {
            gatheringToolConsumetext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 2시간을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
          + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour +2).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        toolName.text = "도끼";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    private void PitGatheing(LanternState lanternstate) //구덩이채집? 
    {
        toolconsumeTime.SetActive(true);
        handconsumeTime.SetActive(true);

        toolitemicon.SetActive(true);
        handitemicon.SetActive(true);

        toolcompleteTime.SetActive(true);
        handcompleteTime.SetActive(true);

        toolbutton.SetActive(true);
        handbutton.SetActive(true);

        toolremainTime.SetActive(true);
        handremainTime.SetActive(true);
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "시 " + "\n"
           + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
             + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 1시간을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
           + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
        }
        else
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 2시간을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
        + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 2).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        toolName.text = "삽";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    private void HerbsGatheing(LanternState lanternstate) //구덩이채집? 
    {
        toolconsumeTime.SetActive(false);
        handconsumeTime.SetActive(true);

        toolitemicon.SetActive(false);
        handitemicon.SetActive(true);

        toolcompleteTime.SetActive(false);
        handcompleteTime.SetActive(true);

        toolbutton.SetActive(false);
        handbutton.SetActive(true);

        toolremainTime.SetActive(false);
        handremainTime.SetActive(true);
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "시 " + "\n"
    + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";

        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
        + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";

        }
        else
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
        }
        toolName.text = "삽";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    private void MushroomGatheing(LanternState lanternstate) //구덩이채집? 
    {
        toolconsumeTime.SetActive(false);
        handconsumeTime.SetActive(true);

        toolitemicon.SetActive(false);
        handitemicon.SetActive(true);

        toolcompleteTime.SetActive(false);
        handcompleteTime.SetActive(true);

        toolbutton.SetActive(false);
        handbutton.SetActive(true);

        toolremainTime.SetActive(false);
        handremainTime.SetActive(true);
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "시 " + "\n"
 + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간을 소비합니다";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "시 " + "\n"
 + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        else
        {
            gatheringToolConsumetext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
     + (Vars.UserData.uData.CurIngameMinute ).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "분";
        }
        toolName.text = "삽";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coWomenMove == null)
        {
            womenbeforePosition = womenplayer.transform.position;
            manbeforePosition = boyPlayer.transform.position;
            boyPlayer.IsCoMove = true;
            if (coWomenMove == null)
            {
                //PlayWalkAnimation();
                PlayWalkAnimationBoy();
            }
            boyPlayer.tag = "Untagged";
            coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, objectPos, 1f, 
                () => { coWomenMove = null; PopUp(); playerAnimationBoy.SetFloat("Speed", 0f); }));
        }
    }
    public void YesTool()
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();

        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByTool();
                break;
            case GatheringObjectType.Pit:
                GatheringPitByTool();
                break;
            case GatheringObjectType.Herbs:
                GatheringHerbsByTool();

                subreward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
                subreward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.subitem);
                gatheringRewardList.Add(subreward.GetComponent<GatheringInDungeonRewardObject>());
                rewardList.Add(curSelectedObj.subitem);
                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByTool();
                break;
            default:
                break;
        }
        // 아이템 획득 준비 보상창에 생성하기

        reward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
        reward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.item);
        gatheringRewardList.Add(reward.GetComponent<GatheringInDungeonRewardObject>());
        rewardList.Add(curSelectedObj.item);
        dungeonrewarddiaryManager.OpenGatheringInDungeonReward();
    }
    public void NoTool()
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();

        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByHand();
                break;
            case GatheringObjectType.Pit:
                GatheringPitByHand();
                break;
            case GatheringObjectType.Herbs:
                GatheringHerbsByHand();
                subreward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
                subreward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.subitem);
                gatheringRewardList.Add(subreward.GetComponent<GatheringInDungeonRewardObject>());
                rewardList.Add(curSelectedObj.subitem);

                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByHand();
                break;
            default:
                break;
        }
        // 아이템 획득준비 보상창에 생성하기
        reward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
        reward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.item);
        gatheringRewardList.Add(reward.GetComponent<GatheringInDungeonRewardObject>());
        rewardList.Add(curSelectedObj.item);
        dungeonrewarddiaryManager.OpenGatheringInDungeonReward();
    }
    public void ClosePopup()
    {
        dungeonrewarddiaryManager.gatheringInDungeonRewardPanel.SetActive(false);
        if (isMove)
        {
            boyPlayer.IsCoMove = true;
            playerAnimationBoy.speed = 0.5f;
            playerAnimationBoy.SetTrigger("Pick");
            dungeonrewarddiaryManager.gameObject.SetActive(false);
            gatheringPanel.SetActive(false);
            Debug.Log("팝업껏다");
            isMove = false;
        }
    }
    private static void GatheringTreeByTool()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(0, 1);
        else
            ConsumeManager.TimeUp(30, 1);
    }
    private static void GatheringPitByTool() // 구덩이
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(0, 1);
        else
            ConsumeManager.TimeUp(30, 1);
    }
    private static void GatheringHerbsByTool() // 약초
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30);
        else
            ConsumeManager.TimeUp(0, 1);
    }
    private static void GatheringMushroomByTool() // 버섯
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30);
        else
            ConsumeManager.TimeUp(0, 1);
    }
 
    public void CloseBagisFull()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
    }
    public void GatheringEnd()
    {
        playerAnimationBoy.speed = 1f;
        if (coWomenMove == null)
        {
            PlayWalkAnimationBoy();
        }
        playerAnimation.SetTrigger("Clap");
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
        Destroy(curSelectedObj.gameObject);
    }
    private static void GatheringTreeByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(0, 1); // 1시간 0분
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30, 1);
        else
            ConsumeManager.TimeUp(0, 2);
    }
    private static void GatheringPitByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(0, 1); // 1시간 0분
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30, 1);
        else
            ConsumeManager.TimeUp(0, 2);
    }
    private static void GatheringHerbsByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30); //30분
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30); //30분
        else
            ConsumeManager.TimeUp(0, 1);
    }
    private static void GatheringMushroomByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
            ConsumeManager.TimeUp(30); //30분
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30); //30분
        else
            ConsumeManager.TimeUp(0, 1);
    }
    private void AfterMove()
    {
        coWomenMove = null;
        womenplayer.transform.rotation = Quaternion.Euler(Vector3.zero);
        womenplayer.CoMoveStop();
        boyPlayer.tag = "Player";
    }
    private void PlayWalkAnimation()
    {
        //playerAnimation.SetTrigger("Walk");
        playerAnimation.SetFloat("Speed", 3f);
    }

    private void PlayWalkAnimationBoy()
    {
        playerAnimationBoy.SetFloat("Speed", 3f);
    }
    public void GetSelectedItem()
    {
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].IsSelect = false;
        }
        if (selecteditemList.Count >0)
        {
            for (int i = 0; i < selecteditemList.Count; i++)
            {
                if (Vars.UserData.AddItemData(selecteditemList[i]) != false)
                {
                    Vars.UserData.AddItemData(selecteditemList[i]);
                    Vars.UserData.ExperienceListAdd(selecteditemList[i].itemId);
                    for (int j = rewardList.Count-1; j>=0 ; j--)
                    {
                        if (rewardList[j] == selecteditemList[i])
                        {
                            rewardList.RemoveAt(j);
                        }
                       
                    }
                    if (rewardList.Count == 0)
                    {
                        rewardList.Clear();
                    }
                }
                else
                {
                    reconfirmPanelManager.gameObject.SetActive(true);
                    reconfirmPanelManager.OpenBagReconfirmInGathering();
                }
            }
           
        }
        dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
        BottomUIManager.Instance.ItemListInit();
        for (int i = 0; i < selecteditemList.Count; i++)
        {
            if (reward != null)
            {
                if (selecteditemList[i] == reward.GetComponent<GatheringInDungeonRewardObject>().Item)
                {
                    Destroy(reward);
                }
            }
            if (subreward != null)
            {
                if (selecteditemList[i] == subreward.GetComponent<GatheringInDungeonRewardObject>().Item)
                {
                    Destroy(subreward);
                }
            }
        }
        for (int i = selecteditemList.Count - 1; i >= 0; i--)
        {
            selecteditemList.RemoveAt(i);
        }
        if (selecteditemList.Count==0)
        {
            selecteditemList.Clear();
        }
    }
    public void GetAllItem()
    {

        for (int i = rewardList.Count - 1; i >= 0; i--)
        {
            if (Vars.UserData.AddItemData(rewardList[i])!=false)
            {
                Vars.UserData.AddItemData(rewardList[i]);
                Vars.UserData.ExperienceListAdd(rewardList[i].itemId);
                rewardList.RemoveAt(i);
            }
            else
            {
                reconfirmPanelManager.gameObject.SetActive(true);
                reconfirmPanelManager.OpenBagReconfirmInGathering();
            }
            dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
            BottomUIManager.Instance.ItemListInit();
        }
        if (reward != null)
        {
            Destroy(reward);
        }
        if (subreward != null)
        {
            Destroy(subreward);
        }
        rewardList.Clear();
        dungeonrewarddiaryManager.gatheringInDungeonRewardPanel.SetActive(false);
        ClosePopup();
    }
}
