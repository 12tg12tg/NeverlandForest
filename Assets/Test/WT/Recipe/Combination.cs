using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private float MakingTime=0f;
/*    public struct Savefortime
    {
        public DateTime startTime;
        public string MakeItem;
    }
    Savefortime save;
    DateTime now;
    DateTime after;*/

    public void Start()
    {
        recipeTable = DataTableManager.GetTable<RecipeDataTable>();

        Debug.Log(Vars.UserData.HaveRecipeIDList.Count);
        CheckCombination.gameObject.SetActive(false);
        result = "";
    }
    private void Update()
    {
        if (CookingStart)
        {   
            MakingTime -= Time.deltaTime;
            //Debug.Log(MakingTime);
        }
        if (MakingTime < 0f)
        {
            MakingTime = 0f;
            if (MakingTime == 0f && CookingStart)
            {
                Debug.Log($"끝났을때 시간 : {DateTime.Now}");
                AfterTimer();
            }
            CookingStart = false;
        }
    }
    void OnGUI()
    {
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
                }

            }
        }
    }

    private void OnEnable()
    {
      /*  SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Time);
        var list = Vars.UserData.MakeList;
        for (int i = 0; i < list.Count; i++)
        {
           
        }*/
    }
    private void OnDisable()
    {
        
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

                Debug.Log($"장비를 시작했을때의 시간 : {DateTime.Now}");
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);
                var second = int.Parse(time[2]);
                MakingTime = hour * 60f * 60f + minute * 60f + second;

                //SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Time);

                var allitem = DataTableManager.GetTable<AllItemDataTable>();
                item = allitem.GetData<AllItemTableElem>(result);
                CheckCombination.gameObject.SetActive(false); // 팝업창 끄고

            }
            else
            {
                Debug.Log("해당 조합법에 맞는 아이템이 없습니다");
            }
        }
    }

    private void AfterTimer()
    {
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
