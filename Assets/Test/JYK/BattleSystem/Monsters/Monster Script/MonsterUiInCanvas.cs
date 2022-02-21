using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUiInCanvas : MonoBehaviour
{
    [Header("최 상단 부모")]
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    public RectTransform rt;

    [HideInInspector] public Vector2 offset2D = new Vector2(0f, 21f);
    [HideInInspector] public Transform targetTr;

    [Header("변경할 이미지, 색, 텍스트")]
    public Sprite range_Near;
    public Sprite range_Far;
    public Sprite token_Sheild;
    public Sprite token_Hp;
    public Image rangeColor;
    public TextMeshProUGUI nextMoveDistance;
    public Image iconImage;

    [Header("프로그래스 바 토큰")]
    public HorizontalLayoutGroup shieldLayoutGroup;
    public HorizontalLayoutGroup hpLayoutGroup;
    [SerializeField] private List<MonsterProgressToken> shields = new List<MonsterProgressToken>();
    [SerializeField] private List<MonsterProgressToken> hps = new List<MonsterProgressToken>();

    [Header("디버프 표시 정보")]
    public List<Image> debuffUIs;

    // Vars
    private bool isInit;
    private bool moveUi;
    public bool UpdateUi { set => moveUi = value; }
    public int ShieldToken
    {
        set
        {
            for (int i = 0; i < shields.Count; ++i)
            {
                if(i < value)
                    shields[i].TokenOn();
                else
                    shields[i].TokenOff();
            }
        }
    }
    public int HpToken
    {
        set
        {
            for (int i = 0; i < hps.Count; ++i)
            {
                if (i < value)
                    hps[i].TokenOn();
                else
                    hps[i].TokenOff();
            }
        }
    }


    public void Init(Transform target, MonsterTableElem monsterElem)
    {
        canvas ??= GetComponentInParent<Canvas>();
        uiCamera ??= canvas.worldCamera;
        rectParent ??= canvas.GetComponent<RectTransform>();

        targetTr = target;
        isInit = true;
        SetOriginalDisplay();
        SetProgress(monsterElem.sheild, monsterElem.hp);
        debuffUIs.ForEach(n => n.enabled = false);
        rangeColor.sprite = (monsterElem.type == MonsterType.Near) ? range_Near : range_Far;
    }

    public void SetOriginalDisplay()
    {
        // 트랜스폼의 스케일, 회전값, 이미지 색상, 알파값 조정.
        nextMoveDistance.alpha = 1f;
        iconImage.color = Color.white;
        iconImage.transform.rotation = Quaternion.identity;
        iconImage.transform.localScale.Set(1f, 1f, 1f);
        nextMoveDistance.transform.localScale.Set(1f, 1f, 1f);
    }

    public void SetProgress(int maxSheildGaugage, int maxHpGaugage)
    {
        shields.Clear();
        for (int i = 0; i < maxSheildGaugage; i++)
        {
            var sheildToken = UIPool.Instance.GetObject(UIPoolTag.ProgressToken);
            Debug.Log($"토큰 {sheildToken.GetInstanceID()}를 {shieldLayoutGroup.GetInstanceID()}에 반환함.");
            var tokenScript = sheildToken.GetComponent<MonsterProgressToken>();
            sheildToken.transform.SetParent(shieldLayoutGroup.transform);
            tokenScript.image.sprite = token_Sheild;
            tokenScript.transform.localScale.Set(1f, 1f, 1f);
            tokenScript.transform.position.Set(0f, 0f, 0f);
            shields.Add(tokenScript);
        }

        hps.Clear();
        for (int i = 0; i < maxHpGaugage; i++)
        {
            var hpToken = UIPool.Instance.GetObject(UIPoolTag.ProgressToken);
            Debug.Log($"토큰 {hpToken.GetInstanceID()}를 {hpLayoutGroup.GetInstanceID()}에 반환함.");
            var tokenScript = hpToken.GetComponent<MonsterProgressToken>();
            hpToken.transform.SetParent(hpLayoutGroup.transform);
            tokenScript.image.sprite = token_Hp;
            tokenScript.transform.localScale.Set(1f, 1f, 1f);
            tokenScript.transform.position.Set(0f, 0f, 0f);
            hps.Add(tokenScript);
        }
    }

    public void Release()
    {
        targetTr = null;
        isInit = false;
        hpLayoutGroup.enabled = true;
        for (int i = 0; i < hps.Count; i++)
        {
            hps[i].TokenOn();
            UIPool.Instance.ReturnObject(UIPoolTag.ProgressToken, hps[i].gameObject);
        }
        for (int i = 0; i < shields.Count; i++)
        {
            shields[i].TokenOn();
            UIPool.Instance.ReturnObject(UIPoolTag.ProgressToken, shields[i].gameObject);
        }
        hps.Clear();
        shields.Clear();
    }

    public void RepositionUi()
    {
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position);

        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out Vector2 localPos);

        rt.localPosition = localPos;
    }

    public void MoveUi()
    {
        RepositionUi();
        RePositionAllUi();
    }

    public void RePositionAllUi()
    {
        var monsters = BattleManager.Instance.monsters;
        var list = from n in monsters
                   where n.State != MonsterState.Dead
                   select n.uiLinker.linkedUi;
        foreach (var ui in list)
        {
            ui.RepositionUi();
        }
    }

    private void Update()
    {
        if (isInit)
        {
            //if (moveUi)
            //{
                RepositionUi();
            //}
        }
    }
}

public class fsefsef
{
    /*
    토큰 -1254398를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254412를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254426를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254440를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254454를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254468를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254314를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254328를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254342를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254356를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254370를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254384를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254566를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254580를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285528를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285542를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285556를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285570를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254482를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254496를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254510를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254524를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254538를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1254552를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285640를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285654를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285668를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285682를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285584를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285598를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285612를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285626를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286032를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286046를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286060를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286074를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:98)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285976를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1285990를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286004를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)

토큰 -1286018를 반환함.
UnityEngine.Debug:Log (object)
MonsterUiInCanvas:SetProgress (int,int) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:111)
MonsterUiInCanvas:Init (UnityEngine.Transform,MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUiInCanvas.cs:77)
MonsterUILinker:SetUI (MonsterTableElem) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:37)
MonsterUILinker:Init (MonsterUnit) (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUILinker.cs:27)
MonsterUnit:Init () (at Assets/Test/JYK/BattleSystem/Monsters/Monster Script/MonsterUnit.cs:102)
BattleManager:FindMonsterToId (int) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:737)
BattleManager:CustomInit () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:425)
BattleManager:Init (bool,bool) (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:173)
BattleManager:Start () (at Assets/Test/JYK/BattleSystem/BattleManager/BattleManager.cs:107)


    */
}