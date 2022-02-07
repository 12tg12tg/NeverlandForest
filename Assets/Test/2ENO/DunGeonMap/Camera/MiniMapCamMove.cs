using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        minimapImg.sizeDelta = new Vector2(200f, 200f);
        minimapImg.anchoredPosition = new Vector2(156f, -133f);
        startSize = minimapImg.sizeDelta;
        startPos = minimapImg.anchoredPosition;
    }

    void Update()
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
            transform.position = new Vector3((leftVec.x + rightVec.x) / 2, 150f, ((topVec.z + bottomVec.z)/ 2) - 5f);
        }
    }

    public void ExpandMinimapToggle()
    {
        var toggle = minimapImg.GetComponent<Toggle>();
        var rect = canvas.GetComponent<RectTransform>().rect;
        var camera = GetComponent<Camera>();

        IsExpand = !toggle.isOn;
        if(IsExpand)
        {
            minimapImg.anchoredPosition = new Vector2(rect.width * 0.5f, -rect.height * 0.5f);
            minimapImg.sizeDelta = new Vector2(rect.width * 0.9f, rect.height * 0.9f);
            camera.fieldOfView = 120f;
        }
        else
        {
            minimapImg.anchoredPosition = startPos;
            minimapImg.sizeDelta = startSize;
            camera.fieldOfView = 60f;
        }
    }
}
//minimapImg.anchoredPosition = !IsExpand ? startPos : new Vector2(rect.width * 0.5f, -rect.height * 0.5f);
//minimapImg.sizeDelta = !IsExpand ? new Vector2(rect.width * 0.2f, rect.height * 0.3f) : new Vector2(rect.width * 0.9f, rect.height * 0.9f);
//camera.fieldOfView = !IsExpand ? 60f : 120f;