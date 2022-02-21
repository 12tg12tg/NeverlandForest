using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleDirecting : MonoBehaviour
{
    [Header("�Ŵ��� ����")]
    [SerializeField] private BattleManager bm;
    [Header("ī�޶� ����")]
    [SerializeField] private Camera battleCamera;
    [SerializeField] private Transform monsterVeiwDest;
    [SerializeField] private Transform battleVeiwDest;

    [Header("��ӱ� ����")]
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

    public List<Material> materials;

    private void Start()
    {
        customShader = Shader.Find("Unlit/MonsterShader2");
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


    // ��� / ���ο� ���� �Լ� =====================================================================
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
        foreach (var tiles in rangedTiles) // �� ���� ����Ʈ ����
        {
            foreach (var unit in tiles.Units)
            {
                if(unit != null && unit is MonsterUnit)
                    layerChangeUnit.Add(unit as MonsterUnit);
            }
        }

        foreach (var monster in monsters) // ����Ʈ ������� �� �Ǵ� ���̴� ����
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
                ren.material.color = new Color(83 / 255f, 88 / 255f, 1f);
            }
        }

        // �÷��̾� �� �߰�
        Utility.ChangeLayerIncludeChildren(attacker.transform, highlightLayer);
    }

    public void ShaderReset()
    {
        // �÷��̾� �� �������
        Utility.ChangeLayerIncludeChildren(attacker.transform, defaultLayer);

        foreach (var monster in layerChangeUnit) // �� �������
        {
            Utility.ChangeLayerIncludeChildren(monster.transform, defaultLayer);
        }

        foreach (var monster in shaderChangeUnit) // ���̴� �������
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


    // ī�޶� �̵� ���� �Լ� =====================================================================
    public void ShowMonsterSide()
    {
        if(bm.isTutorial && !bm.tutorial.tu_01_CamRightButton) // Ʃ�信��
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
        if(bm.isTutorial && !bm.tutorial.tu_02_CamLeftButton) // Ʃ�信��
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

    private void AfterCamComaBack() // iTween �Ϸ� �Լ��� ���
    {
        bm.inputLink.SetActivateStartButton(true);
        UnlockCommand();
        bm.uiLink.ShowArrow(true);
        bm.uiLink.ShowLanternRange();
    }
}
