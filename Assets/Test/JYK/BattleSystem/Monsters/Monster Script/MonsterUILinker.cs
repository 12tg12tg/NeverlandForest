using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class MonsterUILinker : MonoBehaviour
{
    // Instance
    [HideInInspector] public MonsterUiInCanvas linkedUi;
    private Image iconImg;
    private TextMeshProUGUI nextMoveDistance;
    [HideInInspector] public MonsterUnit monsterUnit;
    [SerializeField] private Transform hpBarPos;

    // Vars
    public Sprite attackIcon;
    public Sprite cantMoveIcon;
    public Sprite snareIcon;
    public Sprite bleed1Icon;
    public Sprite bleed2Icon;

    public void Init(MonsterUnit unit)
    {
        monsterUnit = unit;
        SetUI(unit.BaseElem);
    }

    public void SetUI(MonsterTableElem elem)
    {
        var monsterUI = UIPool.Instance.GetObject(UIPoolTag.MonsterUI);
        monsterUI.transform.SetParent(BattleManager.Instance.uiParent);
        linkedUi = monsterUI.GetComponent<MonsterUiInCanvas>();
        nextMoveDistance = linkedUi.nextMoveDistance;
        iconImg = linkedUi.iconImage;
        linkedUi.Init(hpBarPos, elem);
    }
    
    public void Release()
    {
        linkedUi.Release();
        UIPool.Instance.ReturnObject(UIPoolTag.MonsterUI, linkedUi.gameObject);
        linkedUi = null;
    }

    public void UpdateHpBar(int curHp)
    {
        if(linkedUi != null)
            linkedUi.HpToken = curHp;
    }

    public void UpdateSheild(int curS)
    {
        if (linkedUi != null)
            linkedUi.ShieldToken = curS;
    }

    public void UpdateDebuffs(List<ObstacleDebuff> debuffs)
    {
        int count = linkedUi.debuffUIs.Count;
        int num = debuffs.Count;
        for (int i = 0; i < count; i++)
        {
            var img = linkedUi.debuffUIs[i];
            if (i < num)
            {
                img.enabled = true;
                switch (debuffs[i].elem.obstacleType)
                {
                    case TrapTag.Snare:
                        img.sprite = snareIcon;
                        break;
                    case TrapTag.WoodenTrap:
                        img.sprite = bleed1Icon;
                        break;
                    case TrapTag.ThornTrap:
                        img.sprite = bleed2Icon;
                        break;
                }
            }
            else
            {
                img.enabled = false;
            }
        }
    }

    public void UpdateCircleUI(MonsterCommand command)
    {
        switch (command.actionType)
        {
            case MonsterActionType.None:
                nextMoveDistance.enabled = false;
                iconImg.enabled = true;
                iconImg.sprite = cantMoveIcon;
                iconImg.fillAmount = 0f;
                StartCoroutine(FillAmount(iconImg, null));
                break;

            case MonsterActionType.Attack:
                nextMoveDistance.enabled = false;
                iconImg.enabled = true;
                iconImg.sprite = attackIcon;
                break;

            case MonsterActionType.Move:
                nextMoveDistance.enabled = true;
                iconImg.enabled = false;
                nextMoveDistance.text = command.nextMove.ToString();
                StartCoroutine(CoWhenSetCommand(nextMoveDistance.rectTransform, null));
                break;
        }
    }

    public void DisapearCircleUI(MonsterCommand command, UnityAction action)
    {
        switch (command.actionType)
        {
            case MonsterActionType.None:
                StartCoroutine(AlphaDisappear(iconImg,
                    () => { iconImg.color = Color.white; iconImg.enabled = false; action?.Invoke(); }));
                break;

            case MonsterActionType.Attack:
                StartCoroutine(CoWhenAttack(iconImg.rectTransform,
                    () => { iconImg.transform.rotation = Quaternion.identity; iconImg.enabled = false; action?.Invoke(); }));
                break;

            case MonsterActionType.Move:
                StartCoroutine(CoWhenAfterMove(nextMoveDistance.rectTransform, 
                    ()=> { nextMoveDistance.enabled = false; action?.Invoke(); }));
                break;
        }
    }

    public void CantGoAnyWhere(UnityAction action)
    {
        StartCoroutine(AlphaDisappear(nextMoveDistance,
            () => { nextMoveDistance.alpha = 1f; nextMoveDistance.enabled = false; action?.Invoke(); }));
    }

    public void SetCantMove()
    {
        StartCoroutine(Shake(nextMoveDistance.rectTransform,
            () =>
            {
                nextMoveDistance.enabled = false;
                iconImg.sprite = cantMoveIcon;
                iconImg.enabled = true;
                StartCoroutine(FillAmount(iconImg, null));
            }
            ));
    }

    public void FenceAttackDirect(UnityAction action)
    {
        StartCoroutine(CoFenceAttack(action));
    }

    public void AfterFenceAttackDirect(UnityAction action)
    {
        // 3배확대후 비활성화
        StartCoroutine(CoWhenAfterMove(iconImg.rectTransform,
            () => { iconImg.transform.localScale = new Vector3(1f, 1f, 1f); iconImg.transform.rotation = Quaternion.identity; iconImg.enabled = false; action?.Invoke(); }));
    }

    // UI 기능 함수 (코루틴)
    public IEnumerator CoFenceAttack(UnityAction action)
    {
        iconImg.enabled = true;
        nextMoveDistance.enabled = false;
        iconImg.sprite = attackIcon;

        // 공격아이콘 띄우고
        yield return StartCoroutine(Utility.CoScaleChange(iconImg.rectTransform, new Vector3(4f, 4f, 4f), magnificationTime));
        yield return StartCoroutine(Utility.CoScaleChange(iconImg.rectTransform, new Vector3(1f, 1f, 1f), scaleDownTime));

        // 공격아이콘 굴리고
        yield return StartCoroutine(CoWhenAttack(iconImg.rectTransform, action));
    }

    private readonly Vector3 startScale = new Vector3(0.2f, 0.2f, 0.2f);
    private readonly float magnificationTime = 0.3f;
    private readonly float scaleDownTime = 0.2f;
    public IEnumerator CoWhenSetCommand(RectTransform rt, UnityAction action)
    {
        rt.localScale = startScale;
        yield return StartCoroutine(Utility.CoScaleChange(rt, new Vector3(4f, 4f, 4f), magnificationTime));
        yield return StartCoroutine(Utility.CoScaleChange(rt, new Vector3(1f, 1f, 1f), scaleDownTime));
        action?.Invoke();
    }

    public IEnumerator CoWhenAfterMove(RectTransform rt, UnityAction action)
    {
        rt.localScale = new Vector3(1f, 1f, 1f);
        yield return StartCoroutine(Utility.CoScaleChange(rt, new Vector3(3f, 3f, 3f), magnificationTime));
        action?.Invoke();
    }

    private readonly float fadeTime = 0.4f;
    public IEnumerator AlphaDisappear(TextMeshProUGUI text, UnityAction action)
    {
        var startAlpha = text.alpha;
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + fadeTime;
        while(Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / fadeTime;
            var lerp = Mathf.Lerp(startAlpha, 0f, ratio);
            text.alpha = lerp;
            yield return null;
        }
        text.alpha = 0f;
        action?.Invoke();
    }
    public IEnumerator AlphaDisappear(Image img, UnityAction action)
    {
        var originalCOlor = img.color;
        var startAlpha = img.color.a;
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + fadeTime;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / fadeTime;
            var lerp = Mathf.Lerp(startAlpha, 0f, ratio);
            img.color = new Color(originalCOlor.r, originalCOlor.g, originalCOlor.b, lerp);
            yield return null;
        }
        img.color = new Color(originalCOlor.r, originalCOlor.g, originalCOlor.b, 0);
        action?.Invoke();
    }

    private readonly float shakeCycle = 0.05f;
    private readonly float shakeTime = 0.5f;
    private readonly float strength = 0.04f;
    public IEnumerator Shake(RectTransform rt, UnityAction action)
    {
        var pivot = rt.position;
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + shakeTime;
        float timeSave = startTime;
        while (Time.realtimeSinceStartup < endTime)
        {
            var elapsedTime = Time.realtimeSinceStartup - timeSave;
            if(elapsedTime >= shakeCycle)
            {
                timeSave = Time.realtimeSinceStartup;
                Vector2 pos = new Vector2(Random.Range(pivot.x - strength, pivot.x + strength), Random.Range(pivot.y - strength, pivot.y + strength));
                rt.position = pos;
            }
            yield return null;
        }
        rt.localPosition = Vector3.zero;
        action?.Invoke();
    }

    private readonly float fillTime = 0.4f;
    public IEnumerator FillAmount(Image img, UnityAction action)
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + fillTime;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / fillTime;
            var lerp = Mathf.Lerp(0f, 1f, ratio);
            img.fillAmount = lerp;
            yield return null;
        }
        img.fillAmount = 1f;
        action?.Invoke();
    }

    private readonly float changeDegree = 90f;
    private readonly float rotateTime = 0.3f;
    public IEnumerator CoWhenAttack(RectTransform rt, UnityAction action)
    {
        rt.rotation = Quaternion.identity;
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + rotateTime;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / rotateTime;
            var lerp = Mathf.Lerp(0f, changeDegree, ratio);
            rt.rotation = Quaternion.Euler(0f, 0f, lerp);
            yield return null;
        }
        rt.rotation = Quaternion.Euler(0f, 0f, changeDegree);
        yield return new WaitForSeconds(0.2f);
        action?.Invoke();
    }
}
