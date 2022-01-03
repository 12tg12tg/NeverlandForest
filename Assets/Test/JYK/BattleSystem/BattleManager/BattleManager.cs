using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using NewTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerInput
{
    private bool isUpdate;
    public PlayerStats attacker;
    public Vector2 target;
    public DataPlayerSkill skill;
    public PlayerInput(PlayerStats stats)
    {
        attacker = stats;
    }
    public bool IsUpdated { get => isUpdate; }
    public void Clear()
    {
        isUpdate = false;
        attacker = null;
        target = Vector2.zero;
        skill = null;
    }
    public void Create(Vector2 target, DataPlayerSkill skill)
    {
        this.target = target;
        this.skill = skill;
        isUpdate = true;
    }
}


public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Inastance { get => instance; }

    //Unit
    public List<GameObject> enemy = new List<GameObject>();
    public PlayerStats boy;
    public PlayerStats girl;
    private PlayerInput boyInput;
    private PlayerInput girlInput;

    //Canvas
    public CanvasScaler cs;

    //Instance
    public Image dragSlot;
    public BattleSkillInfo info;
    public SkillSelectUI hunterUI;
    public SkillSelectUI herbologistUI;
    public BattleMessage message;
    public BattleFSM FSM;
    private Vector3 belowScreen;
    private float popupHeight;
    private float popupTime = 0.4f;
    private float popdownTime = 0.2f;
    private bool isDrag;

    private SkillSelectUI CurrentOpenUI
    {
        get
        {
            if (hunterUI.gameObject.activeInHierarchy)
                return hunterUI;
            else if (herbologistUI.gameObject.activeInHierarchy)
                return herbologistUI;
            else
                return null;
        }
    }

    private void Awake()
    {       
        instance = this;
        boyInput = new PlayerInput(boy);
        girlInput = new PlayerInput(girl);
    }

    private void Start()
    {     
        //Multitouch 생성
        var multi = MultiTouch.Instance;

        //스킬목록 전달받기
        Init();        

        //스킬창 끌어올리기 위한 수치 계산
        belowScreen = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0f, 0f));
        var rt = hunterUI.GetComponent<RectTransform>();

        float wRatio = Screen.width / cs.referenceResolution.x;
        float hRatio = Screen.height / cs.referenceResolution.y;

        float ratio =
            wRatio * (1f - cs.matchWidthOrHeight) +
            hRatio * (cs.matchWidthOrHeight);

        popupHeight = rt.rect.height * ratio;
        belowScreen.y -= popupHeight;
        hunterUI.transform.position = belowScreen;
        herbologistUI.transform.position = belowScreen;

        //스킬창 Init
        hunterUI.Init(this, Vars.UserData.boySkillList);
        herbologistUI.Init(this, Vars.UserData.girlSkillList);
    }


    private void Update()
    {
        if(isDrag)
        {
            dragSlot.transform.position = MultiTouch.Instance.TouchPos;
        }
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

        //몬스터 리스트 전달받기

        //플레이어 스탯 전달받기
            // Vars 전역 저장소에서 불러오기.
    }    



    public void CreateTempSkillUiForDrag(DataPlayerSkill skill)
    {
        dragSlot.gameObject.SetActive(true);
        dragSlot.sprite = skill.SkillTableElem.IconSprite;
        isDrag = true;
    }

    public void EndTempSkillUiForDrag()
    {
        dragSlot.gameObject.SetActive(false);
        isDrag = false;
    }

    public void PrintMessage(string message, float time, UnityAction action)
    {
        this.message.PrintMessage(message, time, action);
    }
    
    public void OpenSkillInfo(DataPlayerSkill skill, Vector2 pos)
    {
        info.gameObject.SetActive(true);
        info.Init(skill);
        info.transform.position = pos;
    }

    private void OpenSkillSelectUI(SkillSelectUI target)
    {
        var curUI = CurrentOpenUI;
        if (curUI == target)
            return;

        target.gameObject.SetActive(true);
        target.transform.position = belowScreen;
        StartCoroutine(
            CoOpenUp(target.GetComponent<RectTransform>(), curUI?.GetComponent<RectTransform>()));
    }

    private IEnumerator CoCloseDown(RectTransform target)
    {
        float timer = 0f;
        Vector3 startPos = target.position;
        float startY = startPos.y;
        float endY = belowScreen.y;
        while (timer <= popdownTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / popdownTime;
            var lerp = Mathf.Lerp(startY, endY, ratio);
            target.position = new Vector3(startPos.x, lerp, startPos.z);
            yield return null;
        }
        target.position = new Vector3(startPos.x, endY, startPos.z);
        target.gameObject.SetActive(false);
    }

    private IEnumerator CoOpenUp(RectTransform target, RectTransform haveToClose)
    {
        if (haveToClose != null)
        {
            yield return StartCoroutine(CoCloseDown(haveToClose));
        }

        float timer = 0f;
        Vector3 startPos = target.position;
        float startY = startPos.y;
        float endY = startY + popupHeight;
        while (timer <= popupTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / popupTime;
            var lerp = Mathf.Lerp(startY, endY, ratio);
            target.position = new Vector3(startPos.x, lerp, startPos.z);
            yield return null;
        }
        target.position = new Vector3(startPos.x, endY, startPos.z);
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Boy Clicked"))
        {
            OpenSkillSelectUI(hunterUI);
        }
        if (GUILayout.Button("Girl Clicked"))
        {
            OpenSkillSelectUI(herbologistUI);
        }
    }
}
