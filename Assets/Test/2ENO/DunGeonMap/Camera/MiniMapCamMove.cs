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
    private bool IsExpand { get; set; }

    //private Vector3 curRoomPos;

    public void Init()
    {
        startPos = minimapImg.anchoredPosition;
        var rect = canvas.GetComponent<RectTransform>().rect;
        minimapImg.sizeDelta = new Vector2(rect.width * 0.2f, rect.height * 0.3f);
    }

    void Update()
    {
        if (!IsExpand)
        {
            var curObj = DungeonSystem.Instance.dungeonRoomObjectList.Find(x => x.roomIdx == DungeonSystem.Instance.DungeonSystemData.curDungeonRoomData.roomIdx);
            if (curObj != null)
            {
                transform.position = new Vector3(curObj.gameObject.transform.position.x, curObj.gameObject.transform.position.y, -10f);
            }
        }
        else
        {
            transform.position = new Vector3(leftVec.x + rightVec.x / 2, (topVec.y + bottomVec.y / 2) - 5f + 3000f, -10f);
            
        }
    }

    public void ExpandMinimapToggle()
    {
        var toggle = minimapImg.GetComponent<Toggle>();
        var rect = canvas.GetComponent<RectTransform>().rect;
        var camera = GetComponent<Camera>();

        IsExpand = toggle.isOn ? false : true;
        minimapImg.anchoredPosition = toggle.isOn ? startPos : new Vector2(rect.width * 0.5f, -rect.height * 0.5f);
        minimapImg.sizeDelta = toggle.isOn ? new Vector2(rect.width * 0.2f, rect.height * 0.3f) : new Vector2(rect.width * 0.9f, rect.height * 0.9f);
        camera.orthographicSize = toggle.isOn ? 8f : 17f;
    }
}
