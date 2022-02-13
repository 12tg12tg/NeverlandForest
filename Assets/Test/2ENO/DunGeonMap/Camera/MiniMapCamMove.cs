using UnityEngine;
public class MiniMapCamMove : MonoBehaviour
{
    public Vector3 leftVec;
    public Vector3 rightVec;
    public Vector3 topVec;
    public Vector3 bottomVec;
    public RectTransform minimapImg;
    public Canvas canvas;

    private Vector2 startPos;
    private Vector2 startSize;
    private bool IsExpand { get; set; }

    //private Vector3 curRoomPos;

    private void Awake()
    {
        //Init();
    }
  
    public void Init()
    {
        //minimapImg.sizeDelta = new Vector2(200f, 200f);
        //minimapImg.anchoredPosition = new Vector2(156f, -133f);
        //startSize = minimapImg.sizeDelta;
        //startPos = minimapImg.anchoredPosition;
    }

    void Update()
    {
        if (GameManager.Manager.State == GameState.Dungeon)
        {
            if (!IsExpand)
            {
                var curObj = DungeonSystem.Instance.minimapGenerate.dungeonRoomObjectList.Find(x => x.roomIdx == DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx);
                if (curObj != null)
                {
                    transform.position = new Vector3(curObj.gameObject.transform.position.x, 150f, curObj.gameObject.transform.position.z);
                }
            }
            else
            {
                transform.position = new Vector3((leftVec.x + rightVec.x) / 2, 150f, ((topVec.z + bottomVec.z) / 2) - 5f);
            }
        }
    }

    public void ExpandTrue()
    {
        IsExpand = true;
        var cam = GetComponent<Camera>();

        switch(Vars.UserData.mainRoomCount)
        {
            case 4:
                cam.fieldOfView = 90;
                break;
            case 6:
                cam.fieldOfView = 100;
                break;
            case 8:
                cam.fieldOfView = 113;
                break;
        }
        SoundManager.Instance.Play(SoundType.Se_Button);
    }

    public void ExpandFalse()
    {
        IsExpand = false;
        var cam = GetComponent<Camera>();
        cam.fieldOfView = 60;
        SoundManager.Instance.Play(SoundType.Se_Button);
    }
    public void SetMinimapObjectInCamp()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        var list = CampManager.Instance.minimpaGenerate.dungeonRoomObjectList;
        var curObj = list.Find(x => x.roomIdx == CampManager.Instance.CurDungeonRoomIndex);
        if (curObj != null)
        {
            transform.position = new Vector3(curObj.gameObject.transform.position.x, 150f, curObj.gameObject.transform.position.z);
        }
    }

    public void SetMinimapCameraInCamp()
    {
        SoundManager.Instance.Play(SoundType.Se_Button);

        transform.position = new Vector3((leftVec.x + rightVec.x) / 2, 150f, ((topVec.z + bottomVec.z) / 2) - 5f);
    }
}
