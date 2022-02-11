using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DiaryMemoObject : MonoBehaviour
{
    [SerializeField] private Image memoimage;
    public Image MemoImage
    {
        get => memoimage;
        set { memoimage = value; }
    }
    [SerializeField] private TextMeshProUGUI text;
    public TextMeshProUGUI Text
    {
        get => text;
        set { text = value; }
    }
    private DiaryMemo memo;
    private string memoid;
    public string Id
    {
        get => memoid;
        set { memoid = value; }
    }

    public void Init(MemoTable table, string id, DiaryMemo diaryMemo)
    {
        this.memo = diaryMemo;
        memoid = id;
        text.text = table.GetData<MemoTableElem>(id).desc;
    }

    public void ButtonOnClick()
    {
        memo.currentMemo = this;
        memo.OnChangedSelection();
    }
}
