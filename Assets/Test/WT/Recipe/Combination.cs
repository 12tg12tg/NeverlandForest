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
                //CheckCombinationText.text = "���� �ð��� 2:00:00 �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
                if (recipeTable.IsCombine(condiment, material, out result, fire))
                {
                    CheckCombination.gameObject.SetActive(true); // �˾�â ���� 
                    time = recipeTable.IsMakingTime(result); // �ð��޾ƿ��� 
                                                             // time[0] :hour, time[1] : minute time[2] : second
                                                             //�����ǿ� ��ϵǾ��ִ� �������� �ϴ°�� �ð��� �߸鼭 ��������� üũ
                    CheckCombinationText.text = $"���� �ð��� {time[0]}:{time[1]}:{time[2]} �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
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
                inventory.result.sprite = item.IconSprite;
                inventory.resultObject = inventory.itemGoList[int.Parse(result)];

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

        Debug.Log($"makeTime_Hour{makeTime_Hour}");
        Debug.Log($"makeTime_Minute{makeTime_Minute}");

        var RecipeId = recipeTable.GetRecipeId(result);
        var userData = Vars.UserData.HaveRecipeIDList;

        if (!userData.Contains(RecipeId))
        {
            userData.Add(RecipeId);
            info.Init();
            SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Recipe);

            //���߿� ���� ��ȹ���� �����Ͱ� �������� �丮�� �ǰ� ������ �޶�� �����ϱ�
            //���� ���⼭ �˻�� ���ǹ����� �����ǰ� �������� �丮�� ���ش�.
        }
        else
        {
            Debug.Log("�̹� �ش� �����ǰ� ������");
        }
        CookingStart = false;
    }

    public void NoIDontCook()
    {
        CheckCombination.gameObject.SetActive(false); // �˾�â ����
                                                      //������ null�� ����
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
