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
    private PlayerBattleController attacker;

    private Quaternion origianlLanternRotate;
    private Vector3 originalLanternPos;
    private Transform originalLanternParent;
    public Transform lanternGo;
    public Transform lanternLandPos;

    private void Start()
    {
        customShader = Shader.Find("Custom/MonsterShader");
        standardShader = Shader.Find("Standard");

        highlightLayer = LayerMask.NameToLayer("Highlight");
        defaultLayer = LayerMask.NameToLayer("Default");

        origianlColorLight = mainLight.color;

        origianlLanternRotate = lanternGo.localRotation;
        originalLanternPos = lanternGo.localPosition;
        originalLanternParent = lanternGo.parent;
    }

    public void LandDownLantern()
    {
        lanternGo.SetParent(null);
        lanternGo.position = lanternLandPos.position;
        lanternGo.rotation = Quaternion.identity;
    }

    public void HoldLantern()
    {
        lanternGo.SetParent(originalLanternParent);
        lanternGo.localPosition = originalLanternPos;
        lanternGo.localRotation = origianlLanternRotate;
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
        attacker = skill.player == PlayerType.Boy ? bm.boy : bm.girl;

        var monsters = bm.waveLink.AliveMonsters;
        var rangedTiles = TileMaker.Instance.GetSKillTiles(skill);
        foreach (var tiles in rangedTiles) // 빛 받을 리스트 생성
        {
            foreach (var unit in tiles.Units)
            {
                if(unit != null && unit is MonsterUnit)
                    layerChangeUnit.Add(unit as MonsterUnit);
            }
        }

        foreach (var monster in monsters) // 리스트 기반으로 빛 또는 쉐이더 조정
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

        // 플레이어 빛 추가
        Utility.ChangeLayerIncludeChildren(attacker.transform, highlightLayer);
    }

    public void ShaderReset()
    {
        // 플레이어 빛 원래대로
        Utility.ChangeLayerIncludeChildren(attacker.transform, defaultLayer);

        foreach (var monster in layerChangeUnit) // 빛 원래대로
        {
            Utility.ChangeLayerIncludeChildren(monster.transform, defaultLayer);
        }

        foreach (var monster in shaderChangeUnit) // 쉐이더 원래대로
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


    // 카메라 이동 관련 함수 =====================================================================
    public void ShowMonsterSide()
    {
        if(bm.isTutorial && !bm.tutorial.tu_01_CamRightButton) // 튜토에만
        {
            bm.tutorial.tu_01_CamRightButton = true;
        }

        bm.inputLink.SetActivateStartButton(false);
        bm.uiLink.HideArrow();
        bm.uiLink.HideLanternRange();
        LockCommand();
        iTween.MoveTo(battleCamera.gameObject, iTween.Hash(
            "position", monsterVeiwDest.position,
            "time", cameraMovetime,
            "easetype", "easeOutCirc",
            "oncompletetarget", bm.uiLink.gameObject,
            "oncomplete", "ShowArrow",
            "oncompleteparams", false));
    }
    
    public void ShowBattleSide()
    {
        if(bm.isTutorial && !bm.tutorial.tu_02_CamLeftButton) // 튜토에만
        {
            bm.tutorial.tu_02_CamLeftButton = true;
        }

        bm.uiLink.HideArrow();
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
        bm.uiLink.ShowArrow(true);
        bm.uiLink.ShowLanternRange();
    }
}
