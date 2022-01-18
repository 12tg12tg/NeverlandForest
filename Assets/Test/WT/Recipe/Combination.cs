using System.Collections.Generic;
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
    public InventoryController inventoryController;

    public void Start()
    {
        recipeTable = DataTableManager.GetTable<RecipeDataTable>();
        CheckCombination.gameObject.SetActive(false);
        result = "";
    }
    void OnGUI()
    {
        if (GUILayout.Button("Main"))
        {
            SceneManager.LoadScene("JYK_Test_Main");
        }
        if (GUILayout.Button("Start Cooking"))
        {
            if (inventory.fireObject != null)
                fire = inventory.fireObject.DataItem.ItemTableElem.id;
            if (inventory.condimentObject != null)
                condiment = inventory.condimentObject.DataItem.ItemTableElem.id;
            if (inventory.materialObject != null)
                material = inventory.materialObject.DataItem.ItemTableElem.id;

           

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
             
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);
                makeTime_Hour = hour;
                makeTime_Minute = minute;
                var makeTime = 60 * hour + minute;
                var bonFireTime = Vars.UserData.uData.BonfireHour * 60;

                if (bonFireTime > makeTime)
                {
                    CookingStart = true;
                    var allitem = DataTableManager.GetTable<AllItemDataTable>();
                    item = allitem.GetData<AllItemTableElem>(result);
                    inventory.result.sprite = item.IconSprite;
                    inventory.resultObject = inventory.itemGoList[int.Parse(result)];

                    inventory.resultObject.DataItem.dataType = DataType.Material;
                    inventory.resultObject.DataItem.OwnCount = Random.Range(1, 3);
                    inventory.resultObject.DataItem.LimitCount = 5;

                    var list = new List<DataItem>();
                    list.Add(inventory.resultObject.DataItem);
                    inventoryController.OpenChoiceMessageWindow(list);
                    bonFireTime -= makeTime;
                    Vars.UserData.uData.BonfireHour = bonFireTime / 60;
                }
                else
                {
                    Debug.Log("현재 모닥불의 시간이 부족합니다.");
                }
            
                CampManager.Instance.ChangeBonTime();
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
        var RecipeId = recipeTable.GetRecipeId(result);
        var userData = Vars.UserData.HaveRecipeIDList;

        if (!userData.Contains(RecipeId))
        {
            userData.Add(RecipeId);
            info.Init();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);

            //나중에 나온 기획에는 데이터가 있을때만 요리가 되게 변경해 달라고 했으니깐
            //지금 여기서 검사는 조건문으로 레시피가 있을때만 요리를 해준다.
        }
        else
        {
            Debug.Log("이미 해당 레시피가 존재함");
        }
        CookingStart = false;
    }

    public void NoIDontCook()
    {
        CheckCombination.gameObject.SetActive(false); // 팝업창 끄고
                                                      //재료들을 null로 변경
        var allitem = DataTableManager.GetTable<AllItemDataTable>();
        item = allitem.GetData<AllItemTableElem>(0.ToString());

        inventory.fire.sprite = item.IconSprite;
        inventory.fireObject = inventory.itemGoList[0];

        inventory.condiment.sprite = item.IconSprite;
        inventory.condimentObject = null;

        inventory.material.sprite = item.IconSprite;
        inventory.materialObject = null;
    }
}
