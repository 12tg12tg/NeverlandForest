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
    private Animator playerAnimationWomen;
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
    public Sprite nonImage;

    [Header("채집랜더 이미지관련")]
    public PlayerAniControl treePlayer;
    public PlayerAniControl holePlayer;
    public PlayerAniControl flowerPlayer;
    public PlayerAniControl mushPlayer;
    public Camera treeCam;
    public Camera holeCam;
    public Camera flowerCam;
    public Camera mushCam;

    [Header("채집종류 한가지로 변경할때 사용")]
    public GameObject toolConsumeParent;
    public GameObject toolIconParent;
    public GameObject toolCompleteTimeParent;
    public GameObject toolButtonParent;
    public GameObject toolRemainParent;

    private List<GameObject> gatherings = new List<GameObject>();
    [SerializeField]
    private List<GatheringInDungeonRewardObject> gatheringRewardList =
        new List<GatheringInDungeonRewardObject>();
    private List<DataAllItem> rewardList = new List<DataAllItem>();
    public GameObject gatheringParent;
    public List<DataAllItem> selecteditemList = new List<DataAllItem>();
    private static GatheringSystem instance;
    public static GatheringSystem Instance => instance;
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

    public void GatheringRenderCamSet(GatheringObjectType type)
    {
        switch (type)
        {
            case GatheringObjectType.Tree:
                treeCam.gameObject.SetActive(true);
                break;
            case GatheringObjectType.Pit:
                holeCam.gameObject.SetActive(true);
                break;
            case GatheringObjectType.Herbs:
                flowerCam.gameObject.SetActive(true);
                break;
            case GatheringObjectType.Mushroom:
                mushCam.gameObject.SetActive(true);
                break;
        }
    }

    public void GatheringSetAni(GatheringObjectType type, bool isTool = false)
    {
        switch (type)
        {
            case GatheringObjectType.Tree:
                if(isTool)
                    treePlayer.aniControl.SetTrigger("UseAxe");
                else
                    treePlayer.aniControl.SetTrigger("UseHand");
                break;
            case GatheringObjectType.Pit:
                if (isTool)
                    holePlayer.aniControl.SetTrigger("Shovel");
                else
                    holePlayer.aniControl.SetTrigger("DigHand");
                break;
            case GatheringObjectType.Herbs:
                flowerPlayer.aniControl.SetTrigger("PickUp");
                break;
            case GatheringObjectType.Mushroom:
                mushPlayer.aniControl.SetTrigger("HandMush");
                break;
        }
        Debug.Log("애니시작");
    }

    public void GatheringInitAni()
    {
        treePlayer.aniControl.SetTrigger("Idle");
        holePlayer.aniControl.SetTrigger("Idle");
        flowerPlayer.aniControl.SetTrigger("Idle");
        mushPlayer.aniControl.SetTrigger("Idle");
        Debug.Log("애니종료");
    }

    public void GatheringCamOff()
    {
        treeCam.gameObject.SetActive(false);
        holeCam.gameObject.SetActive(false);
        flowerCam.gameObject.SetActive(false);
        mushCam.gameObject.SetActive(false);
    }

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
    private int haveItemCount = 0;

    public void Start()
    {
        instance = this;
        playerAnimationWomen = womenplayer.GetComponent<Animator>();
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
        SoundManager.Instance.Play(SoundType.Se_Button);

        gatheringPanel.SetActive(false);
        ToolPopUp();
    }
    public void NoIDonGathering()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

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
        if (ConsumeManager.CurTimeState == TimeState.DayTime)
        {
            gatheringLanternLeveltext.text = "랜턴" + Vars.UserData.uData.lanternState.ToString();
            if (ConsumeManager.CurLanternState == LanternState.Level3)
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
                GatheringRenderCamSet(GatheringObjectType.Tree);
                TreeGatheing(lanternstate);
                break;
            case GatheringObjectType.Pit:
                GatheringRenderCamSet(GatheringObjectType.Pit);
                PitGatheing(lanternstate);
                break;
            case GatheringObjectType.Herbs:
                GatheringRenderCamSet(GatheringObjectType.Herbs);
                HerbsGatheing(lanternstate);
                break;
            case GatheringObjectType.Mushroom:
                GatheringRenderCamSet(GatheringObjectType.Mushroom);
                MushroomGatheing(lanternstate);
                break;
            default:
                break;
        }
        dungeonrewarddiaryManager.gameObject.SetActive(true);
        dungeonrewarddiaryManager.OpenGatheringInDungeon();
    }

    private void TreeGatheing(LanternState lanternstate)
    {
        toolConsumeParent.SetActive(true);
        toolIconParent.SetActive(true);
        toolCompleteTimeParent.SetActive(true);
        toolButtonParent.SetActive(true);
        toolRemainParent.SetActive(true);

        

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
             + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
           + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "시간은 1시간을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
            + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
        }
        else
        {
            gatheringToolConsumetext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 30분을 소비합니다";
            gatheringHandConsumeText.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 2시간을 소비합니다";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
          + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 2).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
        }
        toolName.text = "도끼";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    private void PitGatheing(LanternState lanternstate) //구덩이채집? 
    {
        toolConsumeParent.SetActive(true);
        toolIconParent.SetActive(true);
        toolCompleteTimeParent.SetActive(true);
        toolButtonParent.SetActive(true);
        toolRemainParent.SetActive(true);

        toolImage.GetComponent<Image>().color = Color.white;
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
       + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
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
        toolImage.sprite = Resources.Load<Sprite>($"Icons/shovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
    }
    private void HerbsGatheing(LanternState lanternstate) //구덩이채집? 
    {
        toolConsumeParent.SetActive(false);
        toolIconParent.SetActive(false);
        toolCompleteTimeParent.SetActive(false);
        toolButtonParent.SetActive(false);
        toolRemainParent.SetActive(false);

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
       + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
        }
        toolName.text = "삽";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/shovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");

      

    }
    private void MushroomGatheing(LanternState lanternstate) //버섯 채집? 
    {
        toolConsumeParent.SetActive(false);
        toolIconParent.SetActive(false);
        toolCompleteTimeParent.SetActive(false);
        toolButtonParent.SetActive(false);
        toolRemainParent.SetActive(false);

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
     + (Vars.UserData.uData.CurIngameMinute).ToString() + "분";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "시 " + "\n"
       + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "분";
        }
        toolName.text = "삽";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/shovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/stick");
      
    }
    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coWomenMove == null)
        {
            womenbeforePosition = womenplayer.transform.position;
            manbeforePosition = boyPlayer.transform.position;
            boyPlayer.IsCoMove = true;
            
            Debug.Log("채집시작");
            if (coWomenMove == null)
            {
                //PlayWalkAnimation();
                PlayWalkAnimationBoy();
                Debug.Log("채집시작222");
            }

            boyPlayer.tag = "Untagged";

            var newPos = new Vector3(objectPos.x-1f, objectPos.y, objectPos.z - 1.5f);

            coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, newPos, 1f,
                () =>
                {
                    Debug.Log("채집종료");
                    coWomenMove = null; PopUp(); playerAnimationBoy.SetFloat("Speed", 0f);
                    if (GameManager.Manager.State == GameState.Tutorial)
                        DungeonSystem.Instance.gatherTutorial.TutorialStep++;
                }));
        }
    }
    public void YesTool()
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        SoundManager.Instance.Play(SoundType.Se_Button);

        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByTool();
                GatheringSetAni(GatheringObjectType.Tree, true);
                break;
            case GatheringObjectType.Pit:
                GatheringPitByTool();
                GatheringSetAni(GatheringObjectType.Pit, true);
                break;
            case GatheringObjectType.Herbs:
                GatheringHerbsByTool();
                GatheringSetAni(GatheringObjectType.Herbs);
                SoundManager.Instance.Play(SoundType.Se_Spade);
                gatheringRewardList[0].Init(curSelectedObj.subitem);
                gatheringRewardList[0].IsHaveItem = true;
                rewardList.Add(gatheringRewardList[0].Item);

                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByTool();
                GatheringSetAni(GatheringObjectType.Mushroom);
                SoundManager.Instance.Play(SoundType.Se_Spade);
                break;
            default:
                break;
        }
        // 아이템 획득 준비 보상창에 생성하기
        if (gatheringRewardList[0].Item != null)
        {
            gatheringRewardList[1].Init(curSelectedObj.subitem);
            gatheringRewardList[1].IsHaveItem = true;
            rewardList.Add(gatheringRewardList[1].Item);
        }
        else
        {
            gatheringRewardList[0].Init(curSelectedObj.item);
            gatheringRewardList[0].IsHaveItem = true;

            rewardList.Add(gatheringRewardList[0].Item);
        }
        dungeonrewarddiaryManager.OpenGatheringInDungeonReward();
    }
    public void NoTool()
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        SoundManager.Instance.Play(SoundType.Se_Button);

        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByHand();
                GatheringSetAni(GatheringObjectType.Tree);
                SoundManager.Instance.Play(SoundType.Se_Hand);

                break;
            case GatheringObjectType.Pit:
                GatheringPitByHand();
                GatheringSetAni(GatheringObjectType.Pit);
                SoundManager.Instance.Play(SoundType.Se_Hand);

                break;
            case GatheringObjectType.Herbs:
                GatheringHerbsByHand();
                GatheringSetAni(GatheringObjectType.Herbs);
                SoundManager.Instance.Play(SoundType.Se_Hand);

                gatheringRewardList[0].Init(curSelectedObj.subitem);
                gatheringRewardList[0].IsHaveItem = true;

                rewardList.Add(gatheringRewardList[0].Item);
                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByHand();
                GatheringSetAni(GatheringObjectType.Mushroom);
                SoundManager.Instance.Play(SoundType.Se_Hand);

                break;
            default:
                break;
        }
        // 아이템 획득준비 보상창에 생성하기
        if (gatheringRewardList[0].Item != null)
        {
            gatheringRewardList[1].Init(curSelectedObj.subitem);
            gatheringRewardList[1].IsHaveItem = true;
            rewardList.Add(gatheringRewardList[1].Item);
        }
        else
        {
            gatheringRewardList[0].Init(curSelectedObj.item);
            gatheringRewardList[0].IsHaveItem = true;

            rewardList.Add(gatheringRewardList[0].Item);
        }
        dungeonrewarddiaryManager.OpenGatheringInDungeonReward();
    }
    public void ClosePopup()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].IsHaveItem)
            {
                haveItemCount++;
            }
        }

        if (haveItemCount == 0)
        {
            reconfirmPanelManager.gameObject.SetActive(false);
            gatheringPanel.SetActive(false);

            if (isMove)
            {
                boyPlayer.IsCoMove = true;

                //switch (curSelectedObj.objectType)
                //{
                //    case GatheringObjectType.Tree:
                //        playerAnimationBoy.SetTrigger("Axe");
                //        SoundManager.Instance.Play(SoundType.Se_Axe);
                //        break;
                //    case GatheringObjectType.Pit:
                //        playerAnimationBoy.SetTrigger("Shovel");
                //        SoundManager.Instance.Play(SoundType.Se_Spade);

                //        break;
                //    case GatheringObjectType.Herbs:
                //        playerAnimationBoy.speed = 0.5f;
                //        playerAnimationBoy.SetTrigger("Pick");
                //        break;
                //    case GatheringObjectType.Mushroom:
                //        playerAnimationBoy.SetTrigger("Hand");
                //        break;
                //}

                GatheringEnd();
                GatheringInitAni();
                GatheringCamOff();
                dungeonrewarddiaryManager.gameObject.SetActive(false);
                Debug.Log("팝업껏다");
                dungeonrewarddiaryManager.gatheringInDungeonRewardPanel.SetActive(false);
                isMove = false;
            }
        }
        else
        {
            reconfirmPanelManager.gameObject.SetActive(true);
            reconfirmPanelManager.rewardNotEmptyPopup.SetActive(true);
        }
        BottomUIManager.Instance.ItemListInit();
    }
    public void YesIfinishGathering()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
        gatheringPanel.SetActive(false);
        SoundManager.Instance.Play(SoundType.Se_Button);

        if (isMove)
        {
            boyPlayer.IsCoMove = true;
            //switch (curSelectedObj.objectType)
            //{
            //    case GatheringObjectType.Tree:
            //        playerAnimationBoy.SetTrigger("Axe");
            //        break;
            //    case GatheringObjectType.Pit:
            //        playerAnimationBoy.SetTrigger("Shovel");
            //        break;
            //    case GatheringObjectType.Herbs:
            //        playerAnimationBoy.speed = 0.5f;
            //        playerAnimationBoy.SetTrigger("Pick");
            //        break;
            //    case GatheringObjectType.Mushroom:
            //        playerAnimationBoy.SetTrigger("Hand");
            //        break;
            //}
            GatheringEnd();
            GatheringInitAni();
            GatheringCamOff();
            dungeonrewarddiaryManager.gameObject.SetActive(false);
            Debug.Log("팝업껏다");
            dungeonrewarddiaryManager.gatheringInDungeonRewardPanel.SetActive(false);
            isMove = false;
        }
        haveItemCount = 0;
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].Item = null;
            gatheringRewardList[i].IsSelect = false;
            gatheringRewardList[i].IsHaveItem = false;
            gatheringRewardList[i].rewardButton.GetComponent<Image>().sprite = nonImage;
        }
    }
    public void GatheringEnd()
    {
        playerAnimationBoy.speed = 1f;
        if (coWomenMove == null)
        {
            PlayWalkAnimationBoy();
        }
        playerAnimationWomen.SetTrigger("Clap");
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
        Destroy(curSelectedObj.gameObject);
    }

    public void GatheringEndFail()
    {
        playerAnimationBoy.speed = 1f;
        if (coWomenMove == null)
        {
            PlayWalkAnimationBoy();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
        Destroy(curSelectedObj.gameObject);
    }

    public void NotYetGathering()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        reconfirmPanelManager.gameObject.SetActive(false);
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
        womenplayer.transform.rotation = Quaternion.Euler(new Vector3(0f,90f,0f));
        boyPlayer.CoMoveStop();
        boyPlayer.tag = "Player";
    }

    private void PlayWalkAnimationBoy()
    {
        playerAnimationBoy.SetFloat("Speed", 3f);
    }
    public void GetSelectedItem()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        if (selecteditemList.Count > 0)
        {
            for (int i = 0; i < selecteditemList.Count; i++)
            {
                if (Vars.UserData.AddItemData(selecteditemList[i]) != false)
                {
                    Vars.UserData.AddItemData(selecteditemList[i]);
                    Vars.UserData.ExperienceListAdd(selecteditemList[i].itemId);
                    for (int j = rewardList.Count - 1; j >= 0; j--)
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
                    reconfirmPanelManager.inventoryFullPopup.SetActive(true);
                }
            }

        }
        dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
        BottomUIManager.Instance.ItemListInit();

        for (int i = selecteditemList.Count - 1; i >= 0; i--)
        {
            selecteditemList.RemoveAt(i);
        }
        if (selecteditemList.Count == 0)
        {
            selecteditemList.Clear();
        }
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].IsSelect)
            {
                gatheringRewardList[i].Item = null;
                gatheringRewardList[i].IsSelect = false;
                gatheringRewardList[i].IsHaveItem = false;
                gatheringRewardList[i].rewardButton.GetComponent<Image>().sprite = nonImage;
            }
        }

    }
    public void GetAllItem()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            if (gatheringRewardList[i].Item != null)
            {
                if (Vars.UserData.AddItemData(gatheringRewardList[i].Item) != false)
                {
                    Vars.UserData.AddItemData(gatheringRewardList[i].Item);
                    Vars.UserData.ExperienceListAdd(gatheringRewardList[i].Item.itemId);
                }
                else
                {
                    reconfirmPanelManager.gameObject.SetActive(true);
                    reconfirmPanelManager.inventoryFullPopup.SetActive(true);

                }
            }
        }
        dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
        BottomUIManager.Instance.ItemListInit();
        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].Item = null;
            gatheringRewardList[i].IsSelect = false;
            gatheringRewardList[i].IsHaveItem = false;
            gatheringRewardList[i].rewardButton.GetComponent<Image>().sprite = nonImage;
        }
    }
}
