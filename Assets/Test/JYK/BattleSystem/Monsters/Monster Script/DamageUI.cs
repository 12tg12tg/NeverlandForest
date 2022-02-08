using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    public enum DamageType { Sheild, Hp }

    [Header("컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rt;

    [Header("상세 설정")]
    [SerializeField] private UIPoolTag poolTag;
    [SerializeField] private Color sheildColor;
    [SerializeField] private Color hpColor;
    [SerializeField] private float endOffsetY = 40f;
    [SerializeField] private float time = 2f;
    [SerializeField] private Vector3 sheild_StartOffset = new Vector3(-15f, 12f, 0f);
    [SerializeField] private Vector3 hp_StartOffset = new Vector3(15f, 6f, 0f);

    public void Init(Vector3 startPos, int num, DamageType type)
    {
        var typeOffset = type == DamageType.Sheild ? sheild_StartOffset : hp_StartOffset;
        rt.localPosition = startPos + typeOffset;
        text.color = type == DamageType.Sheild ? sheildColor : hpColor;
        text.text = $"-{num}";
        StartCoroutine(CoDamageUp());
    }

    private IEnumerator CoDamageUp()
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + time;

        var startPos = rt.localPosition;
        var endPos = startPos + new Vector3(0f, endOffsetY, 0f);
        while (Time.realtimeSinceStartup < endTime)
        {

            var ratio = (Time.realtimeSinceStartup - startTime) / time;

            // 알파러프
            var alpha = Mathf.Lerp(1f, 0f, ratio);
            // 위치러프
            var pos = Vector2.Lerp(startPos, endPos, ratio);
            Debug.Log(pos);

            text.alpha = alpha;
            rt.localPosition = pos;

            yield return null;
        }
        UIPool.Instance.ReturnObject(poolTag, gameObject);
    }
}
