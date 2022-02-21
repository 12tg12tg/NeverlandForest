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
    [Header("�÷��̾�")]
    private Animator playerAnimationWomen;
    private Animator playerAnimationBoy;
    public PlayerDungeonUnit womenplayer;
    public PlayerDungeonUnit boyPlayer;
    [Header("���̾,��Ȯ��")]
    public DungeonRewardDiaryManager dungeonrewarddiaryManager;
    public ReconfirmPanelManager reconfirmPanelManager;
    [Header("ä���ǳڰ���")]
    public GameObject gatheringPanel;
    [Header("ä���ؽ�Ʈ����")]
    public TextMeshProUGUI gatheringtext;
    public TextMeshProUGUI gatheringLanternLeveltext;
    public TextMeshProUGUI gatheringToolConsumetext;
    public TextMeshProUGUI gatheringToolCompleteTimeText;
    public TextMeshProUGUI toolName;
    public TextMeshProUGUI toolCount;
    public TextMeshProUGUI gatheringHandConsumeText;
    public TextMeshProUGUI handCompleteTimeText;

    [Header("ä���̹�������")]
    public Image toolImage;
    public Image handimage;

    [Header("ä����ư����")]
    public GameObject toolbutton;
    public GameObject handbutton;

    [Header("ä����������۰���")]
    public Sprite nonImage;
    [Header("ä������1���� ��")]
    public GameObject toolParent;

    [Header("ä������ �̹�������")]
    public PlayerAniControl treePlayer;
    public PlayerAniControl holePlayer;
    public PlayerAniControl flowerPlayer;
    public PlayerAniControl mushPlayer;
    public Camera treeCam;
    public Camera holeCam;
    public Camera flowerCam;
    public Camera mushCam;

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
    private int axeNum;
    private int shooveNum;
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
                if (isTool)
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
        Debug.Log("�ִϽ���");
    }

    public void GatheringInitAni()
    {
        treePlayer.aniControl.SetTrigger("Idle");
        holePlayer.aniControl.SetTrigger("Idle");
        flowerPlayer.aniControl.SetTrigger("Idle");
        mushPlayer.aniControl.SetTrigger("Idle");
        Debug.Log("�ִ�����");
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
            PlayWalkAnimationBoy();
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
        if (ConsumeManager.CurTimeState == TimeState.DayTime)
        {
            gatheringLanternLeveltext.text = "����" + Vars.UserData.uData.lanternState.ToString();
            if (ConsumeManager.CurLanternState == LanternState.Level3)
                gatheringLanternLeveltext.text += "(30�к�����)";
            else if (ConsumeManager.CurLanternState == LanternState.Level4)
                gatheringLanternLeveltext.text += "(1�ð�������)";
            else
                gatheringLanternLeveltext.text += "(��Ⱑ���� �����������մϴ�)";
        }
        var lanternstate = ConsumeManager.CurLanternState;
        var list = Vars.UserData.HaveAllItemList;
        string axeId = "ITEM_12";
        string shooveId = "ITEM_13";
        bool isaxeHave = false;
        bool isshooveHave = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemId == axeId)
            {
                isaxeHave = true;
                axeNum = i;
            }
            else if (list[i].itemId == shooveId)
            {
                isshooveHave = true;
                shooveNum = i;
            }
        }
        toolbutton.GetComponent<Button>().interactable = false;
        if (isaxeHave)
        {
            switch (curSelectedObj.objectType)
            {
                case GatheringObjectType.Tree:
                    toolbutton.GetComponent<Button>().interactable = true;
                    toolCount.text = list[axeNum].ToolCount.ToString();
                    break;
                case GatheringObjectType.Pit:
                    break;
                case GatheringObjectType.Herbs:
                    break;
                case GatheringObjectType.Mushroom:
                    break;
                default:
                    break;
            }
        }
        else if (isshooveHave)
        {
            switch (curSelectedObj.objectType)
            {
                case GatheringObjectType.Tree:
                    break;
                case GatheringObjectType.Pit:
                    toolbutton.GetComponent<Button>().interactable = true;
                    toolCount.text = list[shooveNum].ToolCount.ToString();
                    break;
                case GatheringObjectType.Herbs:
                    break;
                case GatheringObjectType.Mushroom:
                    break;
                default:
                    break;
            }
        }

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
        if(toolParent != null)
            toolParent.SetActive(true);
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringToolConsumetext.text = "�ð� 30�� �ҿ�" + "\n"
           + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
        + "20���׹̳� �ҿ�";
            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                gatheringToolCompleteTimeText.text =Vars.UserData.uData.CurIngameHour.ToString() + "�� ";
            }
            else
            {
                gatheringToolCompleteTimeText.text =Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
           + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "�ð� 1�ð� �ҿ�" + "\n"
          + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
           + "20���׹̳� �ҿ�"; 
            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
            + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";

            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� ";
            }
            else
            {
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
        }
        else
        {
            gatheringToolConsumetext.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
       + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 2�ð� �ҿ�" + "\n"
           + "20���׹̳� �ҿ�";
            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� ";
            }
            else
            {
                gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 2).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        toolName.text = "����";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/axe");
        handimage.sprite = Resources.Load<Sprite>($"Icons/Hand");
    }
    private void PitGatheing(LanternState lanternstate) //������ä�� 
    {
        if (toolParent != null)
            toolParent.SetActive(true);

        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringToolConsumetext.text = "�ð� 30�� �ҿ�" + "\n"
         + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
        + "20���׹̳� �ҿ�";

            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� ";
            }
            else
            {
                gatheringToolCompleteTimeText.text = Vars.UserData.uData.CurIngameHour.ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringToolConsumetext.text = "�ð� 1�ð� �ҿ�" + "\n"
         + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
           + "20���׹̳� �ҿ�";

            gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
           + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� ";
            }
            else
            {
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
        }
        else
        {
            gatheringToolConsumetext.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
      + "10 ���׹̳� �ҿ�";
            gatheringHandConsumeText.text = "�ð� 2�ð� �ҿ�" + "\n"
           + "20���׹̳� �ҿ�";

            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� ";
            }
            else
            {
                gatheringToolCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour+1).ToString() + "�� " + "\n"
             + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 2).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        toolName.text = "��";
        toolImage.sprite = Resources.Load<Sprite>($"Icons/shovel");
        handimage.sprite = Resources.Load<Sprite>($"Icons/Hand");
    }
    private void HerbsGatheing(LanternState lanternstate) 
    {
        if (toolParent != null)
            toolParent.SetActive(false);

        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
        + "20���׹̳� �ҿ�";

            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";

        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
         + "20���׹̳� �ҿ�";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";

        }
        else
        {
            gatheringHandConsumeText.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
         + "20���׹̳� �ҿ�";
            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� ";
            }
            else
            {
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
        }
        handimage.sprite = Resources.Load<Sprite>($"Icons/Hand");
    }
    private void MushroomGatheing(LanternState lanternstate)
    {
        if (toolParent != null)
            toolParent.SetActive(false);

        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
         + "20���׹̳� �ҿ�";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else if (lanternstate == LanternState.Level3)
        {
            gatheringHandConsumeText.text = "�ð� 1�ð� �ҿ�" + "\n"
        + "20���׹̳� �ҿ�";
            handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
       + (Vars.UserData.uData.CurIngameMinute).ToString() + "��";
        }
        else
        {
            gatheringHandConsumeText.text = "�ð� 1�ð� 30�� �ҿ�" + "\n"
          + "20���׹̳� �ҿ�";
            if ((Vars.UserData.uData.CurIngameMinute + 30) == 60)
            {
                Vars.UserData.uData.CurIngameHour += 1;
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� ";
            }
            else
            {
                handCompleteTimeText.text = (Vars.UserData.uData.CurIngameHour + 1).ToString() + "�� " + "\n"
      + (Vars.UserData.uData.CurIngameMinute + 30).ToString() + "��";
            }
        }
        handimage.sprite = Resources.Load<Sprite>($"Icons/Hand");
    }
    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coWomenMove == null)
        {
            womenbeforePosition = womenplayer.transform.position;
            manbeforePosition = boyPlayer.transform.position;
            boyPlayer.IsCoMove = true;

            if (coWomenMove == null)
                PlayWalkAnimationBoy();
            boyPlayer.tag = "Untagged";
            var newPos = new Vector3(objectPos.x - 1f, objectPos.y, objectPos.z - 1.5f);
            coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, newPos, 1f,
                () =>
                {
                    coWomenMove = null; PopUp(); playerAnimationBoy.SetFloat("Speed", 0f);
                    if (GameManager.Manager.State == GameState.Tutorial)
                        DungeonSystem.Instance.gatherTutorial.NextTutorialStep();
                }));
        }
    }
    public void UseTool() //��������ư
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        SoundManager.Instance.Play(SoundType.Se_Button);
        gatheringRewardList.ForEach(n => n.Init(null));
        var haveItemList = Vars.UserData.HaveAllItemList;
        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByTool();
                GatheringSetAni(GatheringObjectType.Tree, true);
                gatheringRewardList[0].Init(curSelectedObj.item);
                gatheringRewardList[0].IsHaveItem = true;
                rewardList.Add(gatheringRewardList[0].Item);
                SoundManager.Instance.Play(SoundType.Se_Axe);

                haveItemList[axeNum].ToolCount -= 1;
                if (haveItemList[axeNum].ToolCount == 0)
                {
                    haveItemList[axeNum].ToolCount = 5;
                    Vars.UserData.RemoveItemData(haveItemList[axeNum]);
                }

                break;
            case GatheringObjectType.Pit:
                GatheringPitByTool();
                GatheringSetAni(GatheringObjectType.Pit, true);
                SoundManager.Instance.Play(SoundType.Se_Spade);
                gatheringRewardList[0].Init(curSelectedObj.item);
                gatheringRewardList[0].IsHaveItem = true;
                rewardList.Add(gatheringRewardList[0].Item);
                haveItemList[shooveNum].ToolCount -= 1;
                if (haveItemList[shooveNum].ToolCount == 0)
                {
                    haveItemList[shooveNum].ToolCount = 5;
                    Vars.UserData.RemoveItemData(haveItemList[shooveNum]);
                }

                break;
            case GatheringObjectType.Herbs:
                break;
            case GatheringObjectType.Mushroom:
                break;
            default:
                break;
        }
        dungeonrewarddiaryManager.OpenGatheringInDungeonReward();
    }
    public void NoTool() // ���� ������� ����
    {
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        SoundManager.Instance.Play(SoundType.Se_Button);
        gatheringRewardList.ForEach(n => n.Init(null));

        switch (curSelectedObj.objectType)
        {
            case GatheringObjectType.Tree:
                GatheringTreeByHand();
                GatheringSetAni(GatheringObjectType.Tree);
                SoundManager.Instance.Play(SoundType.Se_Hand);
                gatheringRewardList[0].Init(curSelectedObj.subitem);
                if (gatheringRewardList[0].Item != null)
                {
                    gatheringRewardList[0].IsHaveItem = true;
                    rewardList.Add(gatheringRewardList[0].Item);
                }
                break;
            case GatheringObjectType.Pit:
                GatheringPitByHand();
                GatheringSetAni(GatheringObjectType.Pit);
                SoundManager.Instance.Play(SoundType.Se_Hand);
                gatheringRewardList[0].Init(curSelectedObj.subitem);
                if (gatheringRewardList[0].Item != null)
                {
                    gatheringRewardList[0].IsHaveItem = true;
                    rewardList.Add(gatheringRewardList[0].Item);
                }
                break;
            case GatheringObjectType.Herbs:
                GatheringHerbsByHand();
                GatheringSetAni(GatheringObjectType.Herbs);
                SoundManager.Instance.Play(SoundType.Se_Hand);
                gatheringRewardList[0].Init(curSelectedObj.item);
                gatheringRewardList[1].Init(curSelectedObj.subitem);
                if (gatheringRewardList[0].Item != null)
                {
                    gatheringRewardList[0].IsHaveItem = true;
                    rewardList.Add(gatheringRewardList[0].Item);
                }
                if (gatheringRewardList[1].Item != null)
                {
                    gatheringRewardList[1].IsHaveItem = true;
                    rewardList.Add(gatheringRewardList[1].Item);
                }
                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByHand();
                GatheringSetAni(GatheringObjectType.Mushroom);
                SoundManager.Instance.Play(SoundType.Se_Hand);
                gatheringRewardList[0].Init(curSelectedObj.item);
                if (gatheringRewardList[0].Item != null)
                {
                    gatheringRewardList[0].IsHaveItem = true;
                    rewardList.Add(gatheringRewardList[0].Item);
                }
                break;
            default:
                break;
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
        if (isMove)
        {
            boyPlayer.IsCoMove = true;
            GatheringEnd();
            GatheringInitAni();
            GatheringCamOff();
            dungeonrewarddiaryManager.gameObject.SetActive(false);
            Debug.Log("�˾�����");
            dungeonrewarddiaryManager.gatheringInDungeonRewardPanel.SetActive(false);
            haveItemCount = 0;
            isMove = false;
        }
        for (int i = rewardList.Count - 1; i > 0; i--)
        {
            rewardList.RemoveAt(i);
        }
        if (rewardList.Count == 0)
        {
            rewardList.Clear();
            haveItemCount = 0;
        }

        for (int i = 0; i < gatheringRewardList.Count; i++)
        {
            gatheringRewardList[i].Item = null;
        }
        selecteditemList.Clear();

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
            GatheringEnd();
            GatheringInitAni();
            GatheringCamOff();
            dungeonrewarddiaryManager.gameObject.SetActive(false);
            Debug.Log("�˾�����");
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

    public void CloseBagisFull()
    {
        reconfirmPanelManager.gameObject.SetActive(false);
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
        womenplayer.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        boyPlayer.CoMoveStop();
        boyPlayer.tag = "Player";
    }

    private void PlayWalkAnimationBoy()
    {
        playerAnimationBoy.SetFloat("Speed", 3f);
    }

    public void RewardItemLIstInit(List<DataAllItem> itemList)
    {
        gatheringRewardList.ForEach(n => n.Init(null));
        gatheringRewardList.ForEach(n => n.IsSelect = false);

        var count = itemList.Count;

        for (int i = 0; i < count; i++)
        {
            if (i >= gatheringRewardList.Count)
            {
                Debug.LogError("��ġ �̻�!");
            }
            gatheringRewardList[i].Init(itemList[i]);
        }
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

                    if (selecteditemList[i].OwnCount <= 0)
                    {
                        var index = gatheringRewardList.FindIndex(x => x.Item.itemId == selecteditemList[i].itemId);
                        gatheringRewardList[index].IsHaveItem = false;
                        rewardList.RemoveAt(index);
                    }
                    RewardItemLIstInit(rewardList);
                }
                else
                {
                    RewardItemLIstInit(rewardList);
                    reconfirmPanelManager.gameObject.SetActive(true);
                    reconfirmPanelManager.inventoryFullPopup.SetActive(true);
                }
            }
        }



        dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();
        BottomUIManager.Instance.ItemListInit();
    }


    public void GetAllItem()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        var removeList = new List<string>();
        for (int i = 0; i < rewardList.Count; i++)
        {
            if (Vars.UserData.AddItemData(rewardList[i]) != false)
            {
                Vars.UserData.AddItemData(rewardList[i]);
                Vars.UserData.ExperienceListAdd(rewardList[i].itemId);
            }
            dungeonrewarddiaryManager.gatheringInDungeonrewardInventory.ItemButtonInit();

            if (rewardList[i].OwnCount <= 0)
            {
                removeList.Add(rewardList[i].itemId);
            }
        }
        foreach (var id in removeList)
        {
            var index = rewardList.FindIndex(x => x.itemId == id);
            if (index != -1)
                rewardList.RemoveAt(index);
        }
        RewardItemLIstInit(rewardList);

        if (rewardList.Count > 0)
        {
            reconfirmPanelManager.gameObject.SetActive(true);
            reconfirmPanelManager.inventoryFullPopup.SetActive(true);
        }

    }
}
