using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaryMemo : MonoBehaviour
{
    [SerializeField] private List<DiaryMemoObject> itemGoList = new List<DiaryMemoObject>();
    private MemoTable table;
    private AllItemDataTable allitemTable;
    [Header("텍스트 셋팅")]
    public TextMeshProUGUI memoday;
    public TextMeshProUGUI meomodescription;
    [HideInInspector] public DiaryMemoObject currentMemo;

    public void Start()
    {
        allitemTable = DataTableManager.GetTable<AllItemDataTable>();
        Init();
    }
    public void Init()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Memo);
        table = DataTableManager.GetTable<MemoTable>();
        var memoList = Vars.UserData.HaveMemoIDList;

        for (int i = 0; i < 5; i++)
        {
            if (memoList[i] != null)
            {
                itemGoList[i].Init(table, memoList[i], this);
            }
            else
            {
                itemGoList[i].Text.text = string.Empty;  
            }
        }

        memoday.text = "XXXX년 XX월 XX일";
        meomodescription.text = "텍스트가 들어가는 곳입니다.텍스트가 들어가는 곳입니다.텍스트가 들어가는 곳입니다.";

    }

    public void OnChangedSelection()
    {
        memoday.text = table.GetData<MemoTableElem>(currentMemo.Id).date;
        meomodescription.text = table.GetData<MemoTableElem>(currentMemo.Id).desc;
    }
}
