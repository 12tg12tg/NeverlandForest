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

    public Coroutine CoManMove
    {
        get
        {
            return coManMove;
        }
    }

    private Coroutine coWomenMove;
    public Coroutine CoWomenMove
    {
        get
        {
            return coWomenMove;
        }
    }
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

    /* 
    private List<TestGatheringObject> level0 = new List<TestGatheringObject>();
    private List<TestGatheringObject> level1 = new List<TestGatheringObject>();
    private List<TestGatheringObject> level2 = new List<TestGatheringObject>();
    private List<TestGatheringObject> level3 = new List<TestGatheringObject>();
    */

    public void Awake()
    {

    }
    private void Start()
    {
        Init();
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
                //���δ������� ä���� ���۵Ǹ� count 0,1,2����ŭ ���ش�.
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
                     weeds[i].Disappear(); // ó������ ���� ���ΰ�
                 }
                 for (int i = 0; i < count; i++)
                 {
                     weeds[i].Appear();
                 }*/
                break;
            case GatheringType.Path:
                //��ο����� 1���� ������ �Ǳ⶧���� 0,1���� �ϳ��� �������� Ų��.
                //������ 1
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
    public void Init()
    {
        //Utility.arg1Event += manplayer.AnimationChange;
       // Utility.arg0Event += manplayer.CoMoveStop;

        Utility.arg1Event += womenplayer.AnimationChange;
        Utility.arg0Event += womenplayer.CoMoveStop;


        /*  for (int i = 0; i < count; i++)
          {
              if (i % 4 == 0)
                  level0.Add(weeds[i]);
              else if (i % 4 == 1)
                  level1.Add(weeds[i]);
              else if (i % 4 == 2)
                  level2.Add(weeds[i]);
              else if (i % 4 == 3)
                  level3.Add(weeds[i]);
          }*/

    }




    private void OnDestroy()
    {
       // Utility.arg1Event -= manplayer.AnimationChange;
       //Utility.arg0Event -= manplayer.CoMoveStop;

        Utility.arg1Event -= womenplayer.AnimationChange;
        Utility.arg0Event -= womenplayer.CoMoveStop;
    }

    private void Update()
    {

    }
    public void YesIGathering() //���� ä���ϰھ�
    {
        ToolPopUp(); //�ǳ��� ������ 
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
      //  manplayer.isCoMove = true;
        womenplayer.isCoMove = true;

       // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
        //    () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslate2(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, Vector3.zero,
         () => coWomenMove = null));
    }

    private void PopUp()
    {
        Debug.Log("�˾�����");
        gatheringPanel.SetActive(true);
        gatheringtext.gameObject.SetActive(true);
        gatheringtext.text = " ä���� �����ϰڽ��ϱ�";
        Utility.arg0Event -= PopUp;

    }
    private void ToolPopUp()
    {
        Debug.Log("�����˾�����");
        gatheringToolPanel.SetActive(true);
        gatheringTooltext.gameObject.SetActive(true);
        gatheringTooltext.text = "�������� ä���� �Ͻðڽ��ϱ�?";

        if (testLanternLight > 2)
        {
            yesTooltext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "������ 1�ð��� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 30 �Һ��մϴ�" + "\n"
        + "�Ǽ��� 2�ð��� �Һ��մϴ�";
        }
        else
        {
            yesTooltext.text = "������ ���׹̳��� 20 �Һ��մϴ�" + "\n"
         + "������ 2�ð��� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 40 �Һ��մϴ�" + "\n"
           + "�Ǽ��� 3�ð��� �Һ��մϴ�";
        }
    }

    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coManMove == null && coWomenMove == null)
        {
           // manbeforePosition = manplayer.transform.position;
            womenbeforePosition = womenplayer.transform.position;
            Utility.arg0Event += PopUp;
           // manplayer.isCoMove = true;
            womenplayer.isCoMove = true;
           // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, objectPos, 1f, Vector3.zero,
            //    () => coManMove = null));
            coWomenMove ??= StartCoroutine(Utility.CoTranslate2(womenplayer.transform, womenplayer.transform.position, objectPos, 1f, Vector3.zero,
               () => coWomenMove = null));
        }
    }
    public void YesTool()
    {
        var item = curSelectedObj.item;
        if (item !=null)
        {
            Vars.UserData.consumableItemList.Add(item);
        }
       // manplayer.isCoMove = true;
        womenplayer.isCoMove = true;
       // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
        //    () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslate2(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, Vector3.zero,
          () => coWomenMove = null));
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");


        gatheringToolPanel.SetActive(false);
    }
    public void NoTool()
    {
        var item = curSelectedObj.item;
        if (item !=null)
        {
            Vars.UserData.consumableItemList.Add(item);
        }
       // manplayer.isCoMove = true;
        womenplayer.isCoMove = true;
       // coManMove ??= StartCoroutine(Utility.CoTranslate2(manplayer.transform, manplayer.transform.position, manbeforePosition, speed, Vector3.zero,
       //     () => coManMove = null));
        coWomenMove ??= StartCoroutine(Utility.CoTranslate2(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, Vector3.zero,
          () => coWomenMove = null));
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");


        gatheringToolPanel.SetActive(false);
    }

    public void OnGUI()
    {
        /*if (GUILayout.Button("APPEAR"))
        {

        }
        if (GUILayout.Button("LightUp"))
        {
            testLanternLight++;
            Debug.Log("���� �������.");
            Debug.Log(testLanternLight);
        }
        if (GUILayout.Button("LightDown"))
        {
            testLanternLight--;
            Debug.Log("���� ��ο�����.");
            Debug.Log(testLanternLight);
        }*/
    }

}
