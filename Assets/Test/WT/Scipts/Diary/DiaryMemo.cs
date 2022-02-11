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
        for (int i = 0; i < itemGoList.Count; i++)
        {
            itemGoList[i].Text.text = string.Empty;
            itemGoList[i].MemoImage.color = Color.clear;
        }
        memoday.text = string.Empty;
        meomodescription.text = string.Empty;
        for (int i = 0; i < memoList.Count; i++)
        {
            if (memoList[i] != string.Empty)
            {
                itemGoList[i].MemoImage.color = Color.white;
                itemGoList[i].Init(table, memoList[i], this);
            }
        }
    }

    public void OnChangedSelection()
    {
        memoday.text = table.GetData<MemoTableElem>(currentMemo.Id).date;
        meomodescription.text = table.GetData<MemoTableElem>(currentMemo.Id).desc;
    }
}
