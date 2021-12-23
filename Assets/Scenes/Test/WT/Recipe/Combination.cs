using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
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

    private string result ;
    private AllItemTableElem item;
    private string[] time;
    private bool CookingStart = false;
    private float MakingTime = 0f;
    private Dictionary<string, float> WhatMakeWhenMake = new Dictionary<string, float>();
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

        if (MakingTime<0f)
        {
            MakingTime = 0f;
            if (MakingTime == 0f && CookingStart)
            {
                Debug.Log($"�������� �ð� : {DateTime.Now}");
                AfterTimer();
            }
            CookingStart = false;
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("Start Cooking"))
        {
            if (inventory.FireObject !=null)
                fire = inventory.FireObject.DataItem.ItemTableElem.id;
            if (inventory.CondimentObject !=null)
            condiment = inventory.CondimentObject.DataItem.ItemTableElem.id;
            if(inventory.MaterialObject !=null)
            material = inventory.MaterialObject.DataItem.ItemTableElem.id;
            if (fire!=null && condiment !=null && material !=null)
            {
                //CheckCombinationText.text = "���� �ð��� 2:00:00 �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
                if (recipeTable.ISCombine(condiment, material, out result, fire))
                {
                    CheckCombination.gameObject.SetActive(true); // �˾�â ���� 
                    time = recipeTable.IsMakingTime(result); // �ð��޾ƿ��� 
                                                             // time[0] :hour, time[1] : minute time[2] : second
                                                             //�����ǿ� ��ϵǾ��ִ� �������� �ϴ°�� �ð��� �߸鼭 ��������� üũ
                    CheckCombinationText.text = $"���� �ð��� {time[0]}:{time[1]}:{time[2]} �� �Ҹ�˴ϴ�. �������� ���� �Ͻðڽ��ϱ� ? ";
                }
               
            }
        }
    }

    public void YesICook()
    {
        if (Application.internetReachability ==NetworkReachability.NotReachable)
        {
            //���ͳ��� ����Ǿ����� ���� �� �ൿ

        }
   /*   else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            //�����ͷ� ����Ǿ� ���� ���� �ൿ
        }
   */
        else
        {
            //�������̷� ���� �Ǿ� ���� ���� �ൿ (�׳� ���ͳ��� ����Ǿ����� ��)
            if (recipeTable.ISCombine(condiment, material, out result, fire))
            {
                CookingStart = true;
                Debug.Log($"��� ������������ �ð� : {DateTime.Now}");
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);
                var second = int.Parse(time[2]);
                MakingTime = hour * 60f * 60f + minute * 60f + second;

                WhatMakeWhenMake.Add(result, MakingTime);

                var allitem = DataTableManager.GetTable<AllItemDataTable>();
                item = allitem.GetData<AllItemTableElem>(result);
            
                CheckCombination.gameObject.SetActive(false); // �˾�â ����

            }
            else
            {
                Debug.Log("�ش� ���չ��� �´� �������� �����ϴ�");
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
            Debug.Log("�̹� �ش� �����ǰ� ������");
        }
        Debug.Log($"{result}��° ������ {item.name} �� ���Խ��ϴ�");
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
