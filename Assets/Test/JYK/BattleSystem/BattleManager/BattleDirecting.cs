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

    [Header("ȭ��ǥ ��ġ / UI Camera / Canvas")]
    [SerializeField] private Transform arrowPos;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform arrowButton;

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

    private void Start()
    {
        customShader = Shader.Find("Custom/MonsterShader");
        standardShader = Shader.Find("Standard");

        highlightLayer = LayerMask.NameToLayer("Highlight");
        defaultLayer = LayerMask.NameToLayer("Default");

        origianlColorLight = mainLight.color;
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

        var monsters = bm.waveLink.AliveMonsters;
        var rangedTiles = TileMaker.Instance.GetMonsterTiles(skill);
        foreach (var tiles in rangedTiles)
        {
            foreach (var unit in tiles.Units)
            {
                if(unit != null)
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


    // ���� ��ġ ���� �Լ� =====================================================================
    public void ShowArrow(bool buttonForVeiwMonsterSide)
    {
        arrowButton.gameObject.SetActive(true);

        // ���� ī�޶󿡼��� ��ǥ ��ȯ - ��ũ�� ��ǥ ���
        var screenPos = Camera.main.WorldToScreenPoint(arrowPos.position);

        if (screenPos.z < 0.0f)
            screenPos *= -1.0f;

        // ĵ���� �������� ��ũ�� ��ǥ ���ؼ�
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);

        // ĵ���� ���� rect transform�� ������ǥ ����
        arrowButton.localPosition = localPos;

        // ȭ��ǥ ���� ���� �� ���� �Լ� ����
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

    private void AfterCamComaBack() // iTween �Ϸ� �Լ��� ���
    {
        bm.inputLink.SetActivateStartButton(true);
        UnlockCommand();
        ShowArrow(true);
    }
}
