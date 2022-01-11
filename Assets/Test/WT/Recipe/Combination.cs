using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Combination : MonoBehaviour
{
    private string fire;
    private string condiment;
    private string material;
    private RecipeDataTable recipeTable;

    public UICookInventoryList inventory;
    public RecipeObject recipe;
    public RecipeInfo info;
    public GameObject CheckCombination;
    public TextMeshProUGUI CheckCombinationText;

    private string result;
    private AllItemTableElem item;
    private string[] time;
    private bool CookingStart = false;
    private int makeTime_Hour = 0;
    private int makeTime_Minute = 0;

    public struct MadeCooking
    {
        public string Fire;
        public string Condiment;
        public string Material;
        public string Result;
    }
    private MadeCooking madecooking;
    public void Start()
    {
        recipeTable = DataTableManager.GetTable<RecipeDataTable>();

        Debug.Log(Vars.UserData.HaveRecipeIDList.Count);
        CheckCombination.gameObject.SetActive(false);
        result = "";

        madecooking.Fire = string.Empty;
        madecooking.Condiment = string.Empty;
        madecooking.Material = string.Empty;
        madecooking.Result = string.Empty;

    }
    void OnGUI()
    {
        if (GUILayout.Button("Main"))
        {
            SceneManager.LoadScene("JYK_Test_Main");
        }
        if (GUILayout.Button("Start Cooking"))
        {
            if (inventory.FireObject != null)
                fire = inventory.FireObject.DataItem.ItemTableElem.id;
            if (inventory.CondimentObject != null)
                condiment = inventory.CondimentObject.DataItem.ItemTableElem.id;
            if (inventory.MaterialObject != null)
                material = inventory.MaterialObject.DataItem.ItemTableElem.id;
            if (fire != null && condiment != null && material != null)
            {
                //CheckCombinationText.text = "제작 시간은 2:00:00 이 소모됩니다. 아이템을 제작 하시겠습니까 ? ";
                if (recipeTable.IsCombine(condiment, material, out result, fire))
                {
                    CheckCombination.gameObject.SetActive(true); // 팝업창 띄우고 
                    time = recipeTable.IsMakingTime(result); // 시간받아오고 
                                                             // time[0] :hour, time[1] : minute time[2] : second
                                                             //레시피에 등록되어있는 아이템을 하는경우 시간이 뜨면서 만들것인지 체크
                    CheckCombinationText.text = $"제작 시간은 {time[0]}:{time[1]}:{time[2]} 이 소모됩니다. 아이템을 제작 하시겠습니까 ? ";
                    madecooking.Fire = fire;
                    madecooking.Condiment = condiment;
                    madecooking.Material = material;
                    madecooking.Result = result;

                }

            }
        }
      
        if (GUILayout.Button("Start Gesture"))
        {

            SceneManager.LoadScene("Wt_TouchTest");
        }
    }
   
    public void YesICook()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //인터넷이 연결되어있지 않을 때 행동
            //아예 기능을 막아버린다.
        }
        /*   else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
             {
                 //데이터로 연결되어 있을 때의 행동
             }
        */
        else
        {
            //와이파이로 연결 되어 있을 때의 행동 (그냥 인터넷이 연결되어있을 때)
            if (recipeTable.IsCombine(condiment, material, out result, fire))
            {
                CookingStart = true;
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);
                makeTime_Hour = hour;
                makeTime_Minute = minute;
                var allitem = DataTableManager.GetTable<AllItemDataTable>();
                item = allitem.GetData<AllItemTableElem>(result);
                CheckCombination.gameObject.SetActive(false); // 팝업창 끄고
            }
            else
            {
                Debug.Log("해당 조합법에 맞는 아이템이 없습니다");
            }
        }
        if (CookingStart)
        {

            MakeCook();
        }
    }

    private void MakeCook()
    {
        ConsumeManager.TimeUp(makeTime_Minute, makeTime_Hour);
        inventory.Result.sprite = item.IconSprite;
        inventory.ResultObject = inventory.itemGoList[int.Parse(result)];
        var RecipeId = recipeTable.GetRecipeId(result);
        var userData = Vars.UserData.HaveRecipeIDList;
        if (!userData.Contains(RecipeId))
        {
            userData.Add(RecipeId);
            info.Init();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);
        }
        else
        {
            Debug.Log("이미 해당 레시피가 존재함");
        }
        Debug.Log($"{result}번째 아이템 {item.name} 가 나왔습니다");
        Debug.Log(Vars.UserData.uData.CurIngameHour);
        Debug.Log(Vars.UserData.uData.CurIngameMinute);
        CookingStart = false;
    }

    public void NoIDontCook()
    {
        CheckCombination.gameObject.SetActive(false); // 팝업창 끄고
                                                      //재료들을 null로 변경
        inventory.Fire.sprite = null;
        inventory.FireObject = null;

        inventory.Condiment.sprite = null;
        inventory.CondimentObject = null;

        inventory.Material.sprite = null;
        inventory.MaterialObject = null;
    }
}
