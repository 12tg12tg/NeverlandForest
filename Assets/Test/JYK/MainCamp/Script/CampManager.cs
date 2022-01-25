using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
public class CampManager : MonoBehaviour
{
    private static CampManager instance;
    public static CampManager Instance => instance;

    public RoomObject mainRoomPrefab;
    public RoomObject roadPrefab;
    public GameObject mapPos;
    public DiaryManager diaryManager;
    public GameObject newBottomUi;

    public Camera campminimapCamera;
    public GameObject minimapPanel;
    public GameObject diaryRecipePanel;

    private int curDungeonRoomIndex;

    private bool isBlueMoon = false;
    public GameObject bluemoonObject;

    private float recoveryBonTime = 0;
    private float recoverySleepTime = 0;
    private float gatheringTime = 0;
    public float RecoverySleepTime => recoverySleepTime;
    public TextMeshProUGUI bonTimeText;
    public TextMeshProUGUI sleepTimeText;
    public TextMeshProUGUI gatheringTimeText;

    //CampCook
    Vector3 StartPos;
    Vector3 EndPos;
    public GameObject camera;
    public GameObject pot;
    public GameObject workshop;
    public GameObject tent;
    public GameObject bush;

    public GameObject CookPanel;

    public InventoryController inventoryController;
    private DataAllItem itemReward;
    private List<DataAllItem> RewardList = new List<DataAllItem>();
    private bool isCookMove;
    private bool isProduceMove;
    private bool isSleepMove;
    private bool isGatheringMove;

    public SimpleGesture simpleGesture;
    //
    public ReconfirmPanelManager reconfirmPanelManager;
    public enum CampEvent
    {
        StartCook,
        StartGathering,
        StartSleep,
        StartBlueMoon,
        StartMaking,
    }
    public void OnEnable()
    {
        EventBus<CampEvent>.Subscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.Subscribe(CampEvent.StartBlueMoon, OpenBlueMoonScene);
        EventBus<CampEvent>.Subscribe(CampEvent.StartMaking, OpenMaking);
    }
    private void OnDisable()
    {
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartCook, OpenCookScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartGathering, OpenGatheringScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartSleep, StartSleep);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartBlueMoon, OpenBlueMoonScene);
        EventBus<CampEvent>.Unsubscribe(CampEvent.StartMaking, OpenMaking);

        EventBus<CampEvent>.ResetEventBus();
    }
    public void Awake()
    {
        instance = this;
        StartPos = camera.transform.position;
        SetBonTime();
        SetSleepTime();
        SetGatheringTime();
    }
    public void Start()
    {
        GameManager.Manager.SaveLoad.Load(SaveLoadSystem.SaveType.DungeonMap);
        CreateMiniMapObject();
        SetMinimapCamera();
        GameManager.Manager.State = GameState.Camp;
    }
  
    //BonTime
    public void SetBonTime()
    {
        bonTimeText.text = Vars.UserData.uData.BonfireHour.ToString() + "�ð�";
    }
    public void RecoveryBonTime()
    {
        ConsumeManager.RecoveryBonFire(recoveryBonTime);
    }

    //Sleeping
    public void SetSleepTime()
    {
        sleepTimeText.text = recoverySleepTime.ToString() + "��";
    }
    public void PlusSleepTime()
    {
        recoverySleepTime += 30;
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        if (haveMinute< recoverySleepTime)
        {
            recoverySleepTime = haveMinute;
        }
        SetSleepTime();
    }
    public void MinuseSleepTime()
    {
        recoverySleepTime -= 30;
        if (recoverySleepTime<0)
        {
            recoverySleepTime = 0;
        }
        SetSleepTime();
    }
    public void IGoSleep()
    {
        ConsumeManager.RecoveryTiredness();
        recoverySleepTime = 0;
        SetSleepTime();
        SetBonTime();
    }
    public void StartSleep(object[] vals)
    {
        if (vals.Length != 0) return;
        StartSleepInCamp();
    }
    public void StartSleepInCamp()
    {
        var tentPos = tent.transform.position;
        EndPos = new Vector3(tentPos.x, tentPos.y + 2f, tentPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenSleepInCamp));
    }
    public void OpenSleepInCamp()
    {
        isSleepMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenSleeping();
        newBottomUi.SetActive(false);
    }
    public void CloseSleepInCamp()
    {
        if (isSleepMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);
            isSleepMove = false;
        }

    }

    //CookingInCamp
    public void OpenCookScene(object[] vals)
    {
        if (vals.Length != 0) return;
        // SceneManager.LoadScene("Wt_Scene");
        StartCookingInCamp();
    }

    public void StartCookingInCamp()
    {
        var potPos = pot.transform.position;
        EndPos = new Vector3(potPos.x+1f, potPos.y + 2f, potPos.z - 3f);

        StartCoroutine(Utility.CoTranslate(camera.transform,StartPos, EndPos, 3f, OpenCookInCamp));
    }
    public void RotateButtonCheck()
    {
        diaryManager.IsRotation = !diaryManager.IsRotation;
        diaryManager.ChangeRotateButtonImage();
        Debug.Log(diaryManager.IsRotation);
    }

    public void CloseRotationPanel()
    {
        diaryManager.CloseCookingRotation();
    }

    public void CloseCookingCamp()
    {
        if (isCookMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            CloseRotationPanel();
            newBottomUi.SetActive(true);

            isCookMove = false;
        }
    }
    public void OpenCookInCamp()
    {
        isCookMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenCooking();
        newBottomUi.SetActive(false);
    }
    public void StartCooking()
    {
        if (diaryManager.IsRotation)
        {
            simpleGesture.Init();
            diaryManager.OpenCookingRotation();
        }
        else
        {
            if (diaryManager.recipeIcon.fire.sprite != null && 
                diaryManager.recipeIcon.condiment.sprite != null &&
                diaryManager.recipeIcon.material.sprite != null)
            {
                diaryManager.CallMakeCook();
            }
        }
    }
    public void ReCook()
    {
        OpenCookInCamp();
        diaryManager.CloseCookingReward();
    }
    public void CallMakeCook()
    {
        diaryManager.CallMakeCook();
    }

    //GatheringInCamp
    public void OpenGatheringScene(object[] vals)
    {
        if (vals.Length != 0) return;
        StartGatheringInCamp();
    }
    public void StartGatheringInCamp()
    {
        var bushPos = bush.transform.position;
        EndPos = new Vector3(bushPos.x, bushPos.y + 2f, bushPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenGatheringInCamp));
    }
    public void OpenGatheringInCamp()
    {
        isGatheringMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenGatheringInCamp();
        newBottomUi.SetActive(false);
    }
    public void CloseGatheringInCamp()
    {
        if (isGatheringMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);

            isGatheringMove = false;
        }
    }

    //BlueMoonInCamp
    public void OpenBlueMoonScene(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log($"Open Open BlueMoon Scene ");
    }

    //ProducingInCamp
    public  void OpenMaking(object[] vals)
    {
        if (vals.Length != 0) return;
        StartProduceInCamp();
    }
    public void StartProduceInCamp()
    {
        var workPos = workshop.transform.position;
        EndPos = new Vector3(workPos.x, workPos.y + 2f, workPos.z - 5f);
        StartCoroutine(Utility.CoTranslate(camera.transform, StartPos, EndPos, 3f, OpenProduceInCamp));
    }
    public void OpenProduceInCamp()
    {
        isProduceMove = true;
        diaryManager.gameObject.SetActive(true);
        diaryManager.OpenProduce();
        newBottomUi.SetActive(false);
    }
    public void CloseProduceInCamp()
    {
        if (isProduceMove)
        {
            StartCoroutine(Utility.CoTranslate(camera.transform, EndPos, StartPos, 3f));
            newBottomUi.SetActive(true);

            isProduceMove = false;
        }
    }
    //SceneChange
    public void GoWorldMap()
    {
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoDungeon()
    {

        if (Vars.UserData.uData.BonfireHour ==0)
        {
            SceneManager.LoadScene("AS_RandomMap");
        }
        else
        {
            reconfirmPanelManager.OpenBonFireReconfirm();
        }
       
    }
    //MinimapCreate
    public void CreateMiniMapObject()
    {
        curDungeonRoomIndex = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].curDungeonRoomData.roomIdx;
        var array = Vars.UserData.AllDungeonData[Vars.UserData.curDungeonIndex].dungeonRoomArray;

        int curIdx = Vars.UserData.dungeonStartIdx;
        var curRoomIndex = curDungeonRoomIndex;
        while (array[curIdx].nextRoomIdx != -1)
        {
            var room = array[curIdx];
            if (room.RoomType == DunGeonRoomType.MainRoom)
            {
                //var mainRoomPrefab = DungeonSystem.
                var obj = Instantiate(mainRoomPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                if (room.roomIdx == curRoomIndex)
                {
                    var mesh = obj.gameObject.GetComponent<MeshRenderer>();
                    mesh.material.color = Color.blue;
                }
            }
            else
            {
                var obj = Instantiate(roadPrefab, new Vector3(room.Pos.x, room.Pos.y, 0f)
                , Quaternion.identity, mapPos.transform);
                var objectInfo = obj.GetComponent<RoomObject>();
                objectInfo.roomIdx = room.roomIdx;
                if (room.roomIdx == curRoomIndex)
                {
                    var mesh = obj.gameObject.GetComponent<MeshRenderer>();
                    mesh.material.color = Color.blue;
                }
            }
            curIdx = array[curIdx].nextRoomIdx;
        }
        var lastRoom = array[curIdx];
        var lastObj = Instantiate(mainRoomPrefab, new Vector3(lastRoom.Pos.x, lastRoom.Pos.y, 0f)
                     , Quaternion.identity, mapPos.transform);
        var objectInfo2 = lastObj.GetComponent<RoomObject>();
        objectInfo2.roomIdx = lastRoom.roomIdx;
        mapPos.transform.position = mapPos.transform.position + new Vector3(0f, 30f, 0f);
    }
    public void SetMinimapCamera()
    {
        var first = mapPos.transform.GetChild(0);
        var count = mapPos.transform.childCount;
        var lastIdx = count - 1;
        var last = mapPos.transform.GetChild(lastIdx);

        var x = (first.position.x + last.position.x) / 2;

        campminimapCamera.transform.position = new Vector3(x, mapPos.transform.position.y + 10f, -47f);
    }
    public void OnOffBluemoonObject()
    {
        isBlueMoon = !isBlueMoon;
        if (isBlueMoon)
        {
            bluemoonObject.SetActive(true);
        }
        else
        {
            bluemoonObject.SetActive(false);
        }
    }
 
    public void SetRewardItemIcon(Image buttonimage)
    {
        //�����丷: 1 %
        //����: 1 %
        //��������3 %
        //����: 5 %
        // ����: 5 %
        //��: 85 %
        var randNum = Random.Range(1, 101);
      
        var allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        var newItem = new DataAllItem();
        newItem.OwnCount = Random.Range(1, 3);
        newItem.dataType = DataType.AllItem;
        var stringId = $"{randNum}";
        newItem.itemId = randNum;
        newItem.itemTableElem = allitemTable.GetData<AllItemTableElem>(stringId);
        //buttonimage.sprite = newItem.ItemTableElem.IconSprite;
        if (randNum==1)
        {
            //�����丷: 1 %
            itemReward = newItem;
        }
        else if(randNum ==100)
        {
            //����: 1 %
            itemReward = newItem;
        }
        else if(randNum >=2 &&randNum<= 4)
        {
            //��������3 %
            itemReward = newItem;
        }
        else if(randNum >= 5 && randNum <= 9)
        {
            //����: 5 %
            itemReward = newItem;
        }
        else if (randNum >=10 && randNum<= 14)
        {
            // ����: 5 %
            itemReward = newItem;
        }
        else
        {
            //��: 85 %
            Debug.Log("��");
            buttonimage.sprite = Resources.Load<Sprite>($"Icons/xsymbol");
        }
        if (itemReward!=null)
        {
            RewardList.Add(itemReward);
            itemReward = null;
        }
    }
    public void SetGatheringTime()
    {
        gatheringTimeText.text = gatheringTime.ToString() + "��";
    }
    public void PlusGatheringTime()
    {
        gatheringTime += 30;
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        if (haveMinute < gatheringTime)
        {
            gatheringTime = haveMinute;
        }
        SetGatheringTime();
    }
    public void MinusGatheringTime()
    {
        gatheringTime -= 30;
        if (gatheringTime < 0)
        {
            gatheringTime = 0;
        }
        SetGatheringTime();
    }
    public void GatheringInCamp()
    {
        var haveMinute = Vars.UserData.uData.BonfireHour * 60;
        haveMinute -= gatheringTime;
        Vars.UserData.uData.BonfireHour = haveMinute / 60;
        diaryManager.OpenGatheringReward();
        diaryManager.AllRewardClose();
       
        Debug.Log($"gatheringTime{gatheringTime}");
        if (gatheringTime/30==1)//����1��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
        }
        else if (gatheringTime / 30 == 2)//����2��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
            diaryManager.gatheringReward1.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward1_img);
        }
        else if (gatheringTime / 30 == 3)//����3��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
            diaryManager.gatheringReward1.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward1_img);
            diaryManager.gatheringReward2.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward2_img);
        }
        else if (gatheringTime / 30 == 4)//����4��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
            diaryManager.gatheringReward1.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward1_img);
            diaryManager.gatheringReward2.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward2_img);
            diaryManager.gatheringReward3.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward3_img);
        }
        else if (gatheringTime / 30 == 5)//����5��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
            diaryManager.gatheringReward1.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward1_img);
            diaryManager.gatheringReward2.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward2_img);
            diaryManager.gatheringReward3.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward3_img);
            diaryManager.gatheringReward4.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward4_img);
        }
        else if (gatheringTime / 30 == 6)//����6��
        {
            diaryManager.gatheringReward0.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward0_img);
            diaryManager.gatheringReward1.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward1_img);
            diaryManager.gatheringReward2.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward2_img);
            diaryManager.gatheringReward3.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward3_img);
            diaryManager.gatheringReward4.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward4_img);
            diaryManager.gatheringReward5.SetActive(true);
            SetRewardItemIcon(diaryManager.gatheringReward5_img);
        }
        ConsumeManager.TimeUp(gatheringTime);
        if (RewardList.Count !=0)
        {
           
            for (int i = 0; i < RewardList.Count; i++)
            {
                Debug.Log($"itemReward{RewardList[i].ItemTableElem.name}");
            }
        }
        Debug.Log($"���󰹼� : {RewardList.Count}");
        gatheringTime = 0;
        SetGatheringTime();
        SetBonTime();
       
    }

    public void GetItem()
    {

    }
    public void AllItem()
    {
        for (int i = 0; i < RewardList.Count; i++)
        {
            Vars.UserData.AddItemData(RewardList[i]);
        }
    }
}
