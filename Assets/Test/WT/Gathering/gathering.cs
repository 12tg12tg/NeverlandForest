using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class gathering : MonoBehaviour, IPointerClickHandler
{
    public GameObject player;
    public GameObject gatheringPanel;
    public TextMeshProUGUI text;

    private ConsumableTable consumableTable;
    private string id;
    private int rand;
    private Vector3 weedPos;
    private List<DataCunsumable> userconsume;
    private float speed = 3f;
    public void Init()
    {
        //처음 수집씬에 들어오면 랜덤으로 아이템 하나를 들고 시작함
        rand = Random.Range(0, 3);
        id = $"CON_000{rand}";
    }
    private void Awake()
    {
        weedPos = gameObject.transform.position;
        var newweedPos = new Vector3(weedPos.x, weedPos.y, weedPos.z - 2f);
        weedPos = newweedPos;

    }
    private void Start()
    {
        consumableTable = DataTableManager.GetTable<ConsumableTable>();
        userconsume = Vars.UserData.consumableItemList;
        if (userconsume.Count != 0)
        {
            for (int i = 0; i < userconsume.Count; i++)
            {
                Debug.Log(userconsume[i].ItemTableElem.name);
            }
        }
        else
        {
            Debug.Log("비어있다");
        }
        Init();
    }

    private void Update()
    {
        //플레이어 움직이게 하기 
    }
    public void YesIGathering() //좋아 채집하겠어
    {   
        var item = new DataCunsumable();
        item.itemId = rand;
        item.dataType = DataType.Consume;
        item.itemTableElem = consumableTable.GetData<ConsumableTableElem>(id);
        Vars.UserData.consumableItemList.Add(item);

        if (userconsume.Count != 0)
        {
            for (int i = 0; i < userconsume.Count; i++)
            {
                Debug.Log(userconsume[i].ItemTableElem.name);
                Debug.Log($"{userconsume[i].ItemTableElem.name}을 얻었다.");
            }
        }
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("팝업껏다");

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(Utility.CoTranslate(player.transform, player.transform.position, weedPos, speed, PopUp));
    }

    private void PopUp()
    {
        Debug.Log("팝업떳다");
        gatheringPanel.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = " 채집을 시작하겠습니까";
    }
}
