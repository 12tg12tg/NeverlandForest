using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaryMemo : MonoBehaviour
{
    [SerializeField] private List<DiaryMemoObject> itemGoList = new List<DiaryMemoObject>();
    //private MemoTable table;
    private AllItemDataTable allItemTable;
    [Header("�ؽ�Ʈ ����")]
    public TextMeshProUGUI memoday;
    public TextMeshProUGUI meomodescription;
    [HideInInspector] public DiaryMemoObject currentMemo;

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        //SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Memo);

    }
}
