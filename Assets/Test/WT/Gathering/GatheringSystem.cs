using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GatheringSystem : MonoBehaviour
{
    //  public PlayerDungeonUnit manplayer;
    public PlayerDungeonUnit womenplayer;
    public PlayerDungeonUnit boyPlayer;
    public GameObject gatheringPanel;
    public TextMeshProUGUI gatheringtext;

    public GameObject gatheringToolPanel;
    public TextMeshProUGUI gatheringTooltext;
    public TextMeshProUGUI yesTooltext;
    public TextMeshProUGUI noTooltext;

    private List<GameObject> gatherings = new List<GameObject>();

    public int testLanternLight = 0;
    private Animator playerAnimation;
    private Animator playerAnimationBoy;

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

    private void Update()
    {

    }
    public void YesIGathering() //좋아 채집하겠어
    {
        ToolPopUp(); //판넬이 켜지고 
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");
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
        Debug.Log("팝업떳다");
        gatheringPanel.SetActive(true);
        gatheringtext.gameObject.SetActive(true);
        gatheringtext.text = " 채집을 시작하겠습니까";
    }
    private void ToolPopUp()
    {
        Debug.Log("도구팝업떳다");
        gatheringToolPanel.SetActive(true);
        gatheringTooltext.gameObject.SetActive(true);
        gatheringTooltext.text = "무엇으로 채집을 하시겠습니까?";
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
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            yesTooltext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "시간은 1시간을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
        }
        else
        {
            yesTooltext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 2시간을 소비합니다";
        }
    }
    private void PitGatheing(LanternState lanternstate) //구덩이채집? 
    {
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 1시간을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
        }
        else
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 2시간을 소비합니다";
        }
    }
    private void HerbsGatheing(LanternState lanternstate) //구덩이채집? 
    {
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간을 소비합니다";
        }
        else
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
        }
    }
    private void MushroomGatheing(LanternState lanternstate) //구덩이채집? 
    {
        if (lanternstate == LanternState.Level4) // 가장 밝은 상태
        {
            // 1시간의 보정시간을 가진다. 나중에 소비되는 기본 시간값이 나오면 
            // 기본값에서 1시간을 빼준값을 시간으로 소비한다.
            // 스태미나는 랜턴에 영향을 받지않고 똑같이 소비된다.
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
        + "시간은 1시간을 소비합니다";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
          + "시간은 30분을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간을 소비합니다";
        }
        else
        {
            yesTooltext.text = "삽은 스테미나를 10 소비합니다" + "\n"
       + "시간은 1시간 을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 20 소비합니다" + "\n"
           + "시간은 1시간30분을 소비합니다";
        }
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
                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByTool();
                break;
            default:
                break;
        }
        var item = curSelectedObj.item;
        var list = new List<DataItem>();


        // 아이템 획득
        if (item != null)
        {
            item.OwnCount = 1;
            Vars.UserData.AddItemData(item);
            BottomUIManager.Instance.ItemListInit();
            //list.Add(item);
            //inventoryController.OpenChoiceMessageWindow(list);
        }
        boyPlayer.IsCoMove = true;
        playerAnimationBoy.speed = 0.5f;
        playerAnimationBoy.SetTrigger("Pick");
        //if (coWomenMove == null)
        //{
        //    PlayWalkAnimationBoy();
        //}
        //coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");
        gatheringToolPanel.SetActive(false);
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
    public void NoTool()
    {
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
                break;
            case GatheringObjectType.Mushroom:
                GatheringMushroomByHand();
                break;
            default:
                break;
        }
        var item = curSelectedObj.item;
        var list = new List<DataItem>();
        // 아이템 획득
        if (item != null)
        {
            Vars.UserData.AddItemData(item);
            item.OwnCount = 3;
            BottomUIManager.Instance.ItemListInit();
            //list.Add(item);
            //inventoryController.OpenChoiceMessageWindow(list);
        }

        boyPlayer.IsCoMove = true;
        playerAnimationBoy.speed = 0.5f;
        playerAnimationBoy.SetTrigger("Pick");
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");
        gatheringToolPanel.SetActive(false);

        //Vars.UserData.AddItemData(item);
        //list.Add(item);
        //inventoryController.OpenChoiceMessageWindow(list);

        //if (coWomenMove == null)
        //{
        //    //PlayWalkAnimation();
        //    PlayWalkAnimationBoy();
        //}
        //coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(boyPlayer.transform, boyPlayer.transform.position, manbeforePosition, speed, AfterMove));
        //gatheringPanel.SetActive(false);
        //Debug.Log("팝업껏다");
        //gatheringToolPanel.SetActive(false);
        //Destroy(curSelectedObj);

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
}
