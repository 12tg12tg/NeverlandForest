using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public List<GameObject> enemy = new List<GameObject>();
    public List<GameObject> player = new List<GameObject>();

    public BattleMessage message;
    public BattleFSM FSM;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        /*플레이어 스킬 목록 전달받기*/
        var boy = Vars.UserData.boySkillList;
        var girl = Vars.UserData.girlSkillList;

        var skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("0");
        var data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("1");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("2");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("3");
        data = new DataPlayerSkill(skill);
        boy.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("4");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("5");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        skill = DataTableManager.GetTable<PlayerSkillTable>().GetData<PlayerSkillTableElem>("6");
        data = new DataPlayerSkill(skill);
        girl.Add(data);

        /*몬스터 리스트 전달받기*/
    }

    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
    }












    private void OnGUI()
    {
        if(GUILayout.Button("Boy Clicked"))
        {

        }
        if (GUILayout.Button("Girl Clicked"))
        {

        }
        //if (GUILayout.Button("Action"))
        //{
        //    FSM.ChangeState(BattleState.Action);
        //}
        //if (GUILayout.Button("Player Turn"))
        //{
        //    FSM.ChangeState(BattleState.Player);
        //}
        //if (GUILayout.Button("Monster Turn"))
        //{
        //    FSM.ChangeState(BattleState.Monster);
        //}
        //if (GUILayout.Button("Battle Start"))
        //{
        //    FSM.ChangeState(BattleState.Start);
        //}
        //if (GUILayout.Button("Battle End"))
        //{
        //    FSM.ChangeState(BattleState.End);
        //}
    }
}
