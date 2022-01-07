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
                //CheckCombinationText.text = "���� �ð��� 2:00:00 �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
                if (recipeTable.IsCombine(condiment, material, out result, fire))
                {
                    CheckCombination.gameObject.SetActive(true); // �˾�â ���� 
                    time = recipeTable.IsMakingTime(result); // �ð��޾ƿ��� 
                                                             // time[0] :hour, time[1] : minute time[2] : second
                                                             //�����ǿ� ��ϵǾ��ִ� �������� �ϴ°�� �ð��� �߸鼭 ��������� üũ
                    CheckCombinationText.text = $"���� �ð��� {time[0]}:{time[1]}:{time[2]} �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
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
            //���ͳ��� ����Ǿ����� ���� �� �ൿ
            //�ƿ� ����� ���ƹ�����.
        }
        /*   else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
             {
                 //�����ͷ� ����Ǿ� ���� ���� �ൿ
             }
        */
        else
        {
            //�������̷� ���� �Ǿ� ���� ���� �ൿ (�׳� ���ͳ��� ����Ǿ����� ��)
            if (recipeTable.IsCombine(condiment, material, out result, fire))
            {
                CookingStart = true;
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);
                makeTime_Hour = hour;
                makeTime_Minute = minute;
                var allitem = DataTableManager.GetTable<AllItemDataTable>();
                item = allitem.GetData<AllItemTableElem>(result);
                CheckCombination.gameObject.SetActive(false); // �˾�â ����
            }
            else
            {
                Debug.Log("�ش� ���չ��� �´� �������� �����ϴ�");
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
            Debug.Log("�̹� �ش� �����ǰ� ������");
        }
        Debug.Log($"{result}��° ������ {item.name} �� ���Խ��ϴ�");
        Debug.Log(Vars.UserData.CurIngameHour);
        Debug.Log(Vars.UserData.CurIngameMinute);
        CookingStart = false;
    }

    public void NoIDontCook()
    {
        CheckCombination.gameObject.SetActive(false); // �˾�â ����
                                                      //������ null�� ����
        inventory.Fire.sprite = null;
        inventory.FireObject = null;

        inventory.Condiment.sprite = null;
        inventory.CondimentObject = null;

        inventory.Material.sprite = null;
        inventory.MaterialObject = null;
    }
}
