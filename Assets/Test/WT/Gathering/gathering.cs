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
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
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
            Debug.Log("����ִ�");
        }
        Init();
    }

    private void Update()
    {
        //�÷��̾� �����̰� �ϱ� 
    }
    public void YesIGathering() //���� ä���ϰھ�
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
                Debug.Log($"{userconsume[i].ItemTableElem.name}�� �����.");
            }
        }
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(Utility.CoTranslate(player.transform, player.transform.position, weedPos, speed, PopUp));
    }

    private void PopUp()
    {
        Debug.Log("�˾�����");
        gatheringPanel.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = " ä���� �����ϰڽ��ϱ�";
    }
}
