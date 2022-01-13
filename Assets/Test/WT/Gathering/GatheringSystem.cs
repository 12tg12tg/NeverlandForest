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

    public int testLanternLight = 0;
    private Animator playerAnimation;
    public InventoryController inventoryController;
    

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
       playerAnimation = GameObject.FindWithTag("Player").GetComponent<Animator>();
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
                break;
            default:
                break;
        }
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
    public void YesIGathering() //���� ä���ϰھ�
    {
        ToolPopUp(); //�ǳ��� ������ 
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
        womenplayer.IsCoMove = true;
        if (coWomenMove == null)
        {
            PlayWalkAnimation();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
    }

    private void PopUp()
    {
        Debug.Log("�˾�����");
        gatheringPanel.SetActive(true);
        gatheringtext.gameObject.SetActive(true);
        gatheringtext.text = " ä���� �����ϰڽ��ϱ�";
    }
    private void ToolPopUp()
    {
        Debug.Log("�����˾�����");
        gatheringToolPanel.SetActive(true);
        gatheringTooltext.gameObject.SetActive(true);
        gatheringTooltext.text = "�������� ä���� �Ͻðڽ��ϱ�?";
        var lanternstate = ConsumeManager.CurLanternState;
        //switch (gatheringPrehab.GetComponent<GatheringObject>().objectType)
        //{
        //    case GatheringObject.GatheringObjectType.Tree:
        //        TreeGatheing(lanternstate);
        //        break;
        //    case GatheringObject.GatheringObjectType.Pit:
        //        PitGatheing(lanternstate);
        //        break;
        //    case GatheringObject.GatheringObjectType.Herbs:
        //        HerbsGatheing(lanternstate);
        //        break;
        //    case GatheringObject.GatheringObjectType.Mushroom:
        //        MushroomGatheing(lanternstate);
        //        break;
        //    default:
        //        break;
        //}
        TreeGatheing(lanternstate);
    }

    private void TreeGatheing(LanternState lanternstate)
    {
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            yesTooltext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 1�ð��� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
        }
        else
        {
            yesTooltext.text = "������ ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 2�ð��� �Һ��մϴ�";
        }
    }
    private void PitGatheing(LanternState lanternstate) //������ä��? 
    {
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 1�ð��� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
        }
        else
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 2�ð��� �Һ��մϴ�";
        }
    }
    private void HerbsGatheing(LanternState lanternstate) //������ä��? 
    {
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� �� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
        }
    }
    private void MushroomGatheing(LanternState lanternstate) //������ä��? 
    {
        if (lanternstate == LanternState.Level4) // ���� ���� ����
        {
            // 1�ð��� �����ð��� ������. ���߿� �Һ�Ǵ� �⺻ �ð����� ������ 
            // �⺻������ 1�ð��� ���ذ��� �ð����� �Һ��Ѵ�.
            // ���¹̳��� ���Ͽ� ������ �����ʰ� �Ȱ��� �Һ�ȴ�.
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
        + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else if (lanternstate == LanternState.Level3)
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
          + "�ð��� 30���� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð��� �Һ��մϴ�";
        }
        else
        {
            yesTooltext.text = "���� ���׹̳��� 10 �Һ��մϴ�" + "\n"
       + "�ð��� 1�ð� �� �Һ��մϴ�";
            noTooltext.text = "�Ǽ��� ���׹̳��� 20 �Һ��մϴ�" + "\n"
           + "�ð��� 1�ð�30���� �Һ��մϴ�";
        }
    }
    public void GoGatheringObject(Vector3 objectPos)
    {
        if (coWomenMove == null)
        {
            womenbeforePosition = womenplayer.transform.position;
            womenplayer.IsCoMove = true;
            if (coWomenMove == null)
            {
                PlayWalkAnimation();
            }
            coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, objectPos, 1f, 
                () => { coWomenMove = null; PopUp(); }));
        }
    }
    public void YesTool()
    {
        //switch (gatheringPrehab.GetComponent<GatheringObject>().objectType)
        //{
        //    case GatheringObject.GatheringObjectType.Tree:
        //        GatheringTreeByTool();
        //        break;
        //    case GatheringObject.GatheringObjectType.Pit:
        //        GatheringPitByTool();
        //        break;
        //    case GatheringObject.GatheringObjectType.Herbs:
        //        GatheringHerbsByTool();
        //        break;
        //    case GatheringObject.GatheringObjectType.Mushroom:
        //        GatheringMushroomByTool();
        //        break;
        //    default:
        //        break;
        //}
        GatheringTreeByTool();

        var item = curSelectedObj.item;
        var list = new List<DataItem>();
        if (item != null)
        {
            Vars.UserData.AddItemData(item);
            item.OwnCount = 10;
            list.Add(item);
            inventoryController.OpenChoiceMessageWindow(list);
        }
        womenplayer.IsCoMove = true;
        if (coWomenMove == null)
        {
            PlayWalkAnimation();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
        gatheringToolPanel.SetActive(false);
        Destroy(curSelectedObj);
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
    public void NoTool()
    {
        //switch (gatheringPrehab.GetComponent<GatheringObject>().objectType)
        //{
        //    case GatheringObject.GatheringObjectType.Tree:
        //        GatheringTreeByHand();
        //        break;
        //    case GatheringObject.GatheringObjectType.Pit:
        //        GatheringPitByHand();
        //        break;
        //    case GatheringObject.GatheringObjectType.Herbs:
        //        GatheringHerbsByHand();
        //        break;
        //    case GatheringObject.GatheringObjectType.Mushroom:
        //        GatheringMushroomByHand();
        //        break;
        //    default:
        //        break;
        //}
        GatheringTreeByHand();
        var item = curSelectedObj.item;
        var list = new List<DataItem>();

        if (item != null)
        {
            Vars.UserData.AddItemData(item);
            item.OwnCount = 10;
            list.Add(item);
          
            inventoryController.OpenChoiceMessageWindow(list);

        }
        womenplayer.IsCoMove = true;
        if (coWomenMove == null)
        {
            PlayWalkAnimation();
        }
        coWomenMove ??= StartCoroutine(Utility.CoTranslateLookFoward(womenplayer.transform, womenplayer.transform.position, womenbeforePosition, speed, AfterMove));
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
        gatheringToolPanel.SetActive(false);
        Destroy(curSelectedObj);

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
        womenplayer.CoMoveStop();
        womenplayer.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    private void PlayWalkAnimation()
    {
        playerAnimation.SetTrigger("Walk");
    }

}
