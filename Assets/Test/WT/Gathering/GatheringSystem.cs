using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GatheringSystem : MonoBehaviour
{
    //  public PlayerDungeonUnit manplayer;
    public PlayerDungeonUnit womenplayer;
    public GameObject gatheringPanel;
    public TextMeshProUGUI gatheringtext;

    public GameObject gatheringToolPanel;
    public TextMeshProUGUI gatheringTooltext;
    public TextMeshProUGUI yesTooltext;
    public TextMeshProUGUI noTooltext;

    public GameObject gatheringPrehab;
    private List<GameObject> gatherings = new List<GameObject>();
    private Coroutine coManMove;

    public int testLanternLight = 0;

    public enum GatheringType
    {
        MainDunguen,
        Path
    }
    public GatheringType curgatheringType = GatheringType.MainDunguen;

    public Coroutine CoManMove => coManMove;

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

    public void CreateGathering(DunGeonRoomType roomType, GameObject[] objPos, List<DungeonRoom> roomInfoList)
    {
        if (roomType == DunGeonRoomType.MainRoom)
        {
            curgatheringType = GatheringType.MainDunguen;
        }
        else if (roomType == DunGeonRoomType.SubRoom)
        {
            curgatheringType = GatheringType.Path;
        }
        switch (curgatheringType)
        {
            case GatheringType.MainDunguen:

                break;
            case GatheringType.Path:
                for (int i = 0; i < roomInfoList.Count; i++)
                {
                    if (roomInfoList[i].eventList[0] == DunGeonEvent.Gathering)
                    {
                        Setgathering(curgatheringType, objPos[i].transform.position);

                    }
                }
                break;
            default:
                break;
        }

    }
    private void Setgathering(GatheringType gatheringType, Vector3 objPos)
    {
        GameObject eventObj;
        switch (gatheringType)
        {
            case GatheringType.MainDunguen:
                //메인던전에서 채집이 시작되면 count 0,1,2개만큼 켜준다.
                //0~2
                count = Random.Range(0, 3);
                for (int i = 0; i < count; i++)
                {
                    eventObj = Instantiate(gatheringPrehab, objPos, Quaternion.identity);
                    eventObj.AddComponent<GatheringObject>();
                    eventObj.GetComponent<GatheringObject>().Init();
                    eventObj.GetComponent<GatheringObject>().objId = i;
                    gatherings.Add(eventObj);

                }

                /* for (int i = 0; i < weeds.Count; i++)
                 {
                     weeds[i].Disappear(); // 처음에는 전부 꺼두고
                 }
                 for (int i = 0; i < count; i++)
                 {
                     weeds[i].Appear();
                 }*/
                break;
            case GatheringType.Path:
                //통로에서는 1개만 있으면 되기때문에 0,1번중 하나를 랜덤으로 킨다.
                //무조건 1
                count = 1;
                for (int i = 0; i < count; i++)
                {
                    eventObj = Instantiate(gatheringPrehab, objPos, Quaternion.identity);
                    Destroy(eventObj.GetComponent<EventObject>());
                    eventObj.AddComponent<GatheringObject>();
                    GatheringObject obj = eventObj.GetComponent<GatheringObject>();
                    obj.gathering = this;
                    obj.Init();
                    obj.objId = i;
                    gatherings.Add(eventObj);
                }

                //weeds[rand].Appear();
                break;
            default:
                break;
        }
        /*//level0.ForEach(i => i.Appear());
       
        */
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
        //  manplayer.isCoMove = true;
        womenplayer.IsCoMove = true;

        // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
        //    () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
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

        if (testLanternLight > 2)
        {
            yesTooltext.text = "도끼는 스테미나를 10 소비합니다" + "\n"
          + "도끼는 1시간을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 30 소비합니다" + "\n"
        + "맨손은 2시간을 소비합니다";
        }
        else
        {
            yesTooltext.text = "도끼는 스테미나를 20 소비합니다" + "\n"
         + "도끼는 2시간을 소비합니다";
            noTooltext.text = "맨손은 스테미나를 40 소비합니다" + "\n"
           + "맨손은 3시간을 소비합니다";
        }
    }

    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coManMove == null && coWomenMove == null)
        {
            // manbeforePosition = manplayer.transform.position;
            womenbeforePosition = womenplayer.transform.position;
            // manplayer.isCoMove = true;
            womenplayer.IsCoMove = true;
            // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, objectPos, 1f, Vector3.zero,
            //    () => coManMove = null));
            
            coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, objectPos, 1f, 
                () => { coWomenMove = null; PopUp(); }));
        }
    }
    public void YesTool()
    {
        var item = curSelectedObj.item;
        if (item != null)
        {
            Vars.UserData.consumableItemList.Add(item);
        }
        // manplayer.isCoMove = true;
        womenplayer.IsCoMove = true;
        // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
        //    () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");


        gatheringToolPanel.SetActive(false);
    }
    public void NoTool()
    {
        var item = curSelectedObj.item;
        if (item != null)
        {
            Vars.UserData.consumableItemList.Add(item);
        }
        // manplayer.isCoMove = true;
        womenplayer.IsCoMove = true;
        // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
        //     () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");


        gatheringToolPanel.SetActive(false);
    }

    private void AfterMove()
    {
        coWomenMove = null;
        womenplayer.CoMoveStop();
        womenplayer.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void OnGUI()
    {
        /*if (GUILayout.Button("APPEAR"))
        {

        }
        if (GUILayout.Button("LightUp"))
        {
            testLanternLight++;
            Debug.Log("빛이 밝아진다.");
            Debug.Log(testLanternLight);
        }
        if (GUILayout.Button("LightDown"))
        {
            testLanternLight--;
            Debug.Log("빛이 어두워진다.");
            Debug.Log(testLanternLight);
        }*/
    }

}
