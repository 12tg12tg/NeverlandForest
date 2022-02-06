using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleDirecting : MonoBehaviour
{
    [Header("매니저 설정")]
    [SerializeField] private BattleManager bm;
    [Header("카메라 설정")]
    [SerializeField] private Camera battleCamera;
    [SerializeField] private Transform monsterVeiwDest;
    [SerializeField] private Transform battleVeiwDest;

    [Header("화살표 위치 / UI Camera / Canvas")]
    [SerializeField] private Transform arrowPos;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform arrowButton;

    [Header("어둡기 조정")]
    public Light mainLight;
    private const float cameraMovetime = 0.7f;
    private Shader customShader;
    private Shader standardShader;
    private int highlightLayer;
    private int defaultLayer;
    private Color origianlColorLight;
    public Color darknessColorLight;
    private Coroutine coStartBeDark;
    private Coroutine coEndBeLight;
    private const float lightChangeTime = 0.6f;

    private List<MonsterUnit> shaderChangeUnit = new List<MonsterUnit>();
    private List<MonsterUnit> layerChangeUnit = new List<MonsterUnit>();

    private void Start()
    {
        customShader = Shader.Find("Custom/MonsterShader");
        standardShader = Shader.Find("Standard");

        highlightLayer = LayerMask.NameToLayer("Highlight");
        defaultLayer = LayerMask.NameToLayer("Default");

        origianlColorLight = mainLight.color;
    }


    // 어둠 / 슬로우 관련 함수 =====================================================================
    public void SetTimescaleAndShader(PlayerSkillTableElem skill)
    {
        Time.timeScale = 0.4f;
        ShaderChange(skill);

        if (coStartBeDark != null)
        {
            StopCoroutine(coStartBeDark);
            coStartBeDark = null;
        }
        if (coEndBeLight != null)
        {
            StopCoroutine(coEndBeLight);
            coEndBeLight = null;
        }
        var lightColor = mainLight.color;
        coStartBeDark = StartCoroutine(CoLightChange(lightColor, darknessColorLight, lightChangeTime, () => coStartBeDark = null));
    }

    public void ResetTimescaleAndShader()
    {
        Time.timeScale = 1f;
        ShaderReset();

        if (coStartBeDark != null)
        {
            StopCoroutine(coStartBeDark);
            coStartBeDark = null;
        }
        if (coEndBeLight != null)
        {
            StopCoroutine(coEndBeLight);
            coEndBeLight = null;
        }
        var lightColor = mainLight.color;
        coEndBeLight = StartCoroutine(CoLightChange(lightColor, origianlColorLight, lightChangeTime, ()=> coEndBeLight = null));
    }

    public void ShaderChange(PlayerSkillTableElem skill)
    {
        layerChangeUnit.Clear();
        shaderChangeUnit.Clear();

        var monsters = bm.waveLink.AliveMonsters;
        var rangedTiles = TileMaker.Instance.GetSKillTiles(skill);
        foreach (var tiles in rangedTiles)
        {
            foreach (var unit in tiles.Units)
            {
                if(unit != null && unit is MonsterUnit)
                    layerChangeUnit.Add(unit as MonsterUnit);
            }
        }

        foreach (var monster in monsters)
        {
            if(layerChangeUnit.Contains(monster))
            {
                Utility.ChangeLayerIncludeChildren(monster.transform, highlightLayer);
            }
            else
            {
                shaderChangeUnit.Add(monster);
                var ren = monster.mesh;
                ren.material.shader = customShader;
                ren.material.color = Color.gray;
            }
        }
    }

    public void ShaderReset()
    {
        foreach (var monster in layerChangeUnit)
        {
            Utility.ChangeLayerIncludeChildren(monster.transform, defaultLayer);
        }

        foreach (var monster in shaderChangeUnit)
        {
            var ren = monster.mesh;
            ren.material.shader = standardShader;
            ren.material.color = Color.white;
        }

        layerChangeUnit.Clear();
        shaderChangeUnit.Clear();
    }

    public IEnumerator CoLightChange(Color start, Color end, float time, UnityAction action)
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + time;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            var color = Color.Lerp(start, end, ratio);
            mainLight.color = color;
            yield return null;
        }
        mainLight.color = end;
        action?.Invoke();
    }


    // 함정 설치 관련 함수 =====================================================================
    public void ShowArrow(bool buttonForVeiwMonsterSide)
    {
        arrowButton.gameObject.SetActive(true);

        // 월드 카메라에서의 좌표 전환 - 스크린 좌표 얻기
        var screenPos = Camera.main.WorldToScreenPoint(arrowPos.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // 캔버스 기준으로 스크린 좌표 재해석
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // 캔버스 내의 rect transform의 지역좌표 설정
        arrowButton.localPosition = localPos;

        // 화살표 방향 결정 및 연결 함수 관리
        if(!buttonForVeiwMonsterSide)
        {
            arrowButton.rotation = Quaternion.Euler(0f, 0f, 180f);
            arrowButton.GetComponent<Button>().onClick.RemoveAllListeners();
            arrowButton.GetComponent<Button>().onClick.AddListener(ShowBattleSide);
        }
        else
        {
            arrowButton.rotation = Quaternion.identity;
            arrowButton.GetComponent<Button>().onClick.RemoveAllListeners();
            arrowButton.GetComponent<Button>().onClick.AddListener(ShowMonsterSide);
        }
    }

    public void HideArrow()
    {
        arrowButton.gameObject.SetActive(false);
    }

    public void ShowMonsterSide()
    {
        bm.inputLink.SetActivateStartButton(false);
        HideArrow();
        LockCommand();
        iTween.MoveTo(battleCamera.gameObject, iTween.Hash(
            "position", monsterVeiwDest.position,
            "time", cameraMovetime,
            "easetype", "easeOutCirc",
            "oncompletetarget", gameObject,
            "oncomplete", "ShowArrow",
            "oncompleteparams", false));
    }
    
    public void ShowBattleSide()
    {
        HideArrow();
        iTween.MoveTo(battleCamera.gameObject, iTween.Hash(
            "position", battleVeiwDest.position,
            "time", cameraMovetime,
            "easetype", "easeOutCirc",
            "oncompletetarget", gameObject,
            "oncomplete", "AfterCamComaBack"));
    }

    private void LockCommand()
    {
        BottomUIManager.Instance.ItemButtonsInteractive(false);
    }

    private void UnlockCommand()
    {
        BottomUIManager.Instance.ItemButtonsInteractive(true);
    }

    private void AfterCamComaBack() // iTween 완료 함수로 사용
    {
        bm.inputLink.SetActivateStartButton(true);
        UnlockCommand();
        ShowArrow(true);
    }
}
