using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GatheringSystem : MonoBehaviour
{
    //  public PlayerDungeonUnit manplayer;
    public PlayerDungeonUnit womenplayer;
    public PlayerDungeonUnit boyPlayer;

    public DiaryManager diaryManager;

    public GameObject gatheringPanel;
    public TextMeshProUGUI gatheringtext;

    public GameObject gatheringToolPanel;
    public TextMeshProUGUI gatheringTooltext;

    private List<GameObject> gatherings = new List<GameObject>();

    public int testLanternLight = 0;
    private Animator playerAnimation;
    private Animator playerAnimationBoy;

    //New
    public TextMeshProUGUI gatheringLanternLeveltext;
    public TextMeshProUGUI gatheringToolConsumetext;
    public Image toolImage;
    public TextMeshProUGUI gatheringToolCompleteTimeText;
    public TextMeshProUGUI toolName;
    public TextMeshProUGUI toolCount;

    public TextMeshProUGUI gatheringHandConsumeText;
    public Image handimage;
    public TextMeshProUGUI handCompleteTimeText;
    public ReconfirmPanelManager reconfirmPanelManager;
    
    public GameObject toolconsumeTime;
    public GameObject handconsumeTime;

    public GameObject toolitemicon;
    public GameObject handitemicon;

    public GameObject toolcompleteTime;
    public GameObject handcompleteTime;

    public GameObject toolbutton;
    public GameObject handbutton;

    public GameObject toolremainTime;
    public GameObject handremainTime;

    public GameObject gatheringInDungeonRewardObject;
    private List<GatheringInDungeonRewardObject> gatheringRewardList = 
        new List<GatheringInDungeonRewardObject>();
    private List<DataAllItem> rewardList = new List<DataAllItem>();
    public GameObject gatheringParent;

    private DataAllItem selecteditem;
    public DataAllItem SelectedItem
    {
        get=> selecteditem;
        set
        {
            selecteditem = value;
        }
    }
    private static GatheringSystem instance;
    public static GatheringSystem Instance => instance;
    GameObject reward;
    GameObject subreward;


    public void Awake()
    {
        instance = this;
    }
    private bool isMove;
    public enum GatheringType
    {
        MainDunguen,
        Path
    }
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

    public void YesIGathering() //���� ä���ϰھ�
    {
        ToolPopUp(); //�ǳ��� ������ 
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        boyPlayer.IsCoMove = true;
        if (coWomenMove == null)
        {
            //PlayWalkAnimation();
            PlayWalkAnimationBoy();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
    }

    private void PopUp()
    {
        gatheringPanel.SetActive(true);
        gatheringtext.gameObject.SetActive(true);
        gatheringtext.text = " ä���� �����ϰڽ��ϱ�";
        isMove = true;
    }
    private void ToolPopUp()
    {
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenGatheringInDungeon();
        if (ConsumeManager.CurTimeState ==TimeState.DayTime)
        {
            gatheringLanternLeveltext.text = "����" + Vars.UserData.uData.lanternState.ToString();
            if (ConsumeManager.CurLanternState ==LanternState.Level3)
            {
                gatheringLanternLeveltext.text += "(30�к�����)";
            }
            else if (ConsumeManager.CurLanternState == LanternState.Level4)
            {
                gatheringLanternLeveltext.text += "(1�ð�������)";
            }
            else
            {
                gatheringLanternLeveltext.text += "(��Ⱑ���� �����������մϴ�)";

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

        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
           gatheringToolConsumetext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� " + "\n"
           + (Vars.UserData.uData.CurIngameMinute ).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� " + "\n"
            + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
        }
        else
        {
            gatheringToolConsumetext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 2�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
          + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour +2).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        toolName.text = "����";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/gloves");
    }
    private void PitGatheing(LanternState lanternstate) //������ä��? 
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
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
           + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
           + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
        }
        else
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 2�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
        + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 2).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        toolName.text = "��";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/BrokenShovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/gloves");
    }
    private void HerbsGatheing(LanternState lanternstate) //������ä��? 
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
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
    + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";

        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
        + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";

        }
        else
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� �� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
        }
        toolName.text = "��";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/BrokenShovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/gloves");
    }
    private void MushroomGatheing(LanternState lanternstate) //������ä��? 
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
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
 + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð��� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
 + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else
        {
            gatheringToolConsumetext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� �� �Һ��մϴ�";
            gatheringHandConsumeText.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
     + (Vars.UserData.uData.CurIngameMinute ).ToString() + "��";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute+30).ToString() + "��";
        }
        toolName.text = "��";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/BrokenShovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/gloves");
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
        // ������ ȹ�� �غ� ����â�� �����ϱ�

        reward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
        reward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.item);
        gatheringRewardList.Add(reward.GetComponent<GatheringInDungeonRewardObject>());
        rewardList.Add(curSelectedObj.item);
        diaryManager.OpenGatheringInDungeonReward();
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
        // ������ ȹ���غ� ����â�� �����ϱ�
        reward = Instantiate(gatheringInDungeonRewardObject, gatheringParent.transform);
        reward.GetComponent<GatheringInDungeonRewardObject>().Init(curSelectedObj.item);
        gatheringRewardList.Add(reward.GetComponent<GatheringInDungeonRewardObject>());
        rewardList.Add(curSelectedObj.item);
        diaryManager.OpenGatheringInDungeonReward();
    }
    public void ClosePopup()
    {
        if (isMove)
        {
            boyPlayer.IsCoMove = true;
            playerAnimationBoy.speed = 0.5f;
            playerAnimationBoy.SetTrigger("Pick");
            diaryManager.gameObject.SetActive(false);
            gatheringPanel.SetActive(false);
            Debug.Log("�˾�����");
            gatheringToolPanel.SetActive(false);
            isMove = false;
            diaryManager.gatheringInDungeonReward.SetActive(false);
        }
    }
    private static void GatheringTreeByTool()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(0, 1);
        else
            ConsumeManager.TimeUp(30, 1);
    }
    private static void GatheringPitByTool() // ������
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(0, 1);
        else
            ConsumeManager.TimeUp(30, 1);
    }
    private static void GatheringHerbsByTool() // ����
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(30);
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30);
        else
            ConsumeManager.TimeUp(0, 1);
    }
    private static void GatheringMushroomByTool() // ����
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
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
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(0, 1); // 1�ð� 0��
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30, 1);
        else
            ConsumeManager.TimeUp(0, 2);
    }
    private static void GatheringPitByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(0, 1); // 1�ð� 0��
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30, 1);
        else
            ConsumeManager.TimeUp(0, 2);
    }
    private static void GatheringHerbsByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(30); //30��
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30); //30��
        else
            ConsumeManager.TimeUp(0, 1);
    }
    private static void GatheringMushroomByHand()
    {
        var lanternstate = ConsumeManager.CurLanternState;
        if (lanternstate == LanternState.Level4) // ���� ���� ����
            ConsumeManager.TimeUp(30); //30��
        else if (lanternstate == LanternState.Level3)
            ConsumeManager.TimeUp(30); //30��
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
        if (selecteditem != null)
        {
            if (Vars.UserData.AddItemData(selecteditem) != false)
            {
                Vars.UserData.AddItemData(selecteditem);
                for (int i = 0; i < rewardList.Count; i++)
                {
                    if (rewardList[i] ==selecteditem)
                    {
                        rewardList.RemoveAt(i);
                    }
                }
            }
            else
            {
                reconfirmPanelManager.gameObject.SetActive(true);
                reconfirmPanelManager.OpenBagReconfirm();
            }
        }
        diaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
        BottomUIManager.Instance.ItemListInit();
        if (reward!=null)
        {
            if (selecteditem == reward.GetComponent<GatheringInDungeonRewardObject>().Item)
            {   
                Destroy(reward);
            }
        }
      
        if (subreward!=null)
        {
            if (selecteditem == subreward.GetComponent<GatheringInDungeonRewardObject>().Item)
            {
                Destroy(subreward);
            }
        }
        
        selecteditem = null;
    }
    public void GetAllItem()
    {
        for (int i = 0; i < rewardList.Count; i++)
        {
            if (Vars.UserData.AddItemData(rewardList[i]) != false)
            {
                Vars.UserData.AddItemData(rewardList[i]);
                rewardList.RemoveAt(i);
            }
            else
            {
                reconfirmPanelManager.gameObject.SetActive(true);
                reconfirmPanelManager.OpenBagReconfirm();
            }
            diaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
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
        ClosePopup();
    }
}
