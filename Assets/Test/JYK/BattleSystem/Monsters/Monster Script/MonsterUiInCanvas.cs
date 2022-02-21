using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterUiInCanvas : MonoBehaviour
{
    [Header("�� ��� �θ�")]
    private Camera uiCamera;
    private Canvas canvas;
    private RectTransform rectParent;
    public RectTransform rt;

    [HideInInspector] public Vector2 offset2D = new Vector2(0f, 21f);
    [HideInInspector] public Transform targetTr;

    [Header("������ �̹���, ��, �ؽ�Ʈ")]
    public Sprite range_Near;
    public Sprite range_Far;
    public Sprite token_Sheild;
    public Sprite token_Hp;
    public Image rangeColor;
    public TextMeshProUGUI nextMoveDistance;
    public Image iconImage;

    [Header("���α׷��� �� ��ū")]
    public HorizontalLayoutGroup shieldLayoutGroup;
    public HorizontalLayoutGroup hpLayoutGroup;
    [SerializeField] private List<MonsterProgressToken> shields = new List<MonsterProgressToken>();
    [SerializeField] private List<MonsterProgressToken> hps = new List<MonsterProgressToken>();

    [Header("����� ǥ�� ����")]
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
        // Ʈ�������� ������, ȸ����, �̹��� ����, ���İ� ����.
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
            Debug.Log($"��ū {sheildToken.GetInstanceID()}�� {shieldLayoutGroup.GetInstanceID()}�� ��ȯ��.");
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
            Debug.Log($"��ū {hpToken.GetInstanceID()}�� {hpLayoutGroup.GetInstanceID()}�� ��ȯ��.");
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
    ��ū -1254398�� ��ȯ��.
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

��ū -1254412�� ��ȯ��.
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

��ū -1254426�� ��ȯ��.
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

��ū -1254440�� ��ȯ��.
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

��ū -1254454�� ��ȯ��.
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

��ū -1254468�� ��ȯ��.
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

��ū -1254314�� ��ȯ��.
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

��ū -1254328�� ��ȯ��.
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

��ū -1254342�� ��ȯ��.
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

��ū -1254356�� ��ȯ��.
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

��ū -1254370�� ��ȯ��.
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

��ū -1254384�� ��ȯ��.
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

��ū -1254566�� ��ȯ��.
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

��ū -1254580�� ��ȯ��.
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

��ū -1285528�� ��ȯ��.
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

��ū -1285542�� ��ȯ��.
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

��ū -1285556�� ��ȯ��.
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

��ū -1285570�� ��ȯ��.
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

��ū -1254482�� ��ȯ��.
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

��ū -1254496�� ��ȯ��.
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

��ū -1254510�� ��ȯ��.
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

��ū -1254524�� ��ȯ��.
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

��ū -1254538�� ��ȯ��.
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

��ū -1254552�� ��ȯ��.
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

��ū -1285640�� ��ȯ��.
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

��ū -1285654�� ��ȯ��.
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

��ū -1285668�� ��ȯ��.
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

��ū -1285682�� ��ȯ��.
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

��ū -1285584�� ��ȯ��.
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

��ū -1285598�� ��ȯ��.
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

��ū -1285612�� ��ȯ��.
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

��ū -1285626�� ��ȯ��.
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

��ū -1286032�� ��ȯ��.
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

��ū -1286046�� ��ȯ��.
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

��ū -1286060�� ��ȯ��.
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

��ū -1286074�� ��ȯ��.
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

��ū -1285976�� ��ȯ��.
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

��ū -1285990�� ��ȯ��.
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

��ū -1286004�� ��ȯ��.
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

��ū -1286018�� ��ȯ��.
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