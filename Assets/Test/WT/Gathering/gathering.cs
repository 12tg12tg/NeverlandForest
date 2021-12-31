using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class Gathering : MonoBehaviour
{
    private Coroutine coMove;
    public PlayerDungeonUnit player;
    public GameObject gatheringPanel;
    public TextMeshProUGUI text;

    private ConsumableTable consumableTable;
    private string id;
    private int rand;
    private Vector3 weedPos;
    private List<DataCunsumable> userconsume;
    private float speed = 3f;

    private Vector3 beforePosition;
    public void Init()
    {
        //ó�� �������� ������ �������� ������ �ϳ��� ��� ������
        rand = Random.Range(0, 3);
        id = $"CON_000{rand}";
        Utility.arg1Event += player.AnimationChange;
        Utility.arg0Event += player.CoMoveStop;
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

    private void OnDestroy()
    {
        Utility.arg1Event -= player.AnimationChange;
        Utility.arg0Event -= player.CoMoveStop;
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
        player.isCoMove = true;
        coMove ??= StartCoroutine(Utility.CoTranslate2(player.transform, player.transform.position, beforePosition, speed, Vector3.zero,
            () => coMove = null));
    }
    public void NoIDonGathering()
    {
        gatheringPanel.SetActive(false);
        Debug.Log("�˾�����");
        player.isCoMove = true;
        coMove ??= StartCoroutine(Utility.CoTranslate2(player.transform, player.transform.position, beforePosition, speed, Vector3.zero,
            () => coMove = null));
    }

    private void PopUp()
    {
        Debug.Log("�˾�����");
        gatheringPanel.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = " ä���� �����ϰڽ��ϱ�";
        Utility.arg0Event -= PopUp;
    }

    public void GoGatheringObject(Vector3 objectPos)
    {
        beforePosition = player.transform.position;
        Utility.arg0Event += PopUp;
        player.isCoMove = true;
        coMove ??= StartCoroutine(Utility.CoTranslate2(player.transform, player.transform.position, objectPos, 1f, Vector3.zero,
            () => coMove = null));
    }
}
