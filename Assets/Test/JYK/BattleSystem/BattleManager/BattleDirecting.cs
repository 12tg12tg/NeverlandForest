using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private const float cameraMovetime = 0.7f;



    // ��� / ���ο� ���� �Լ� =====================================================================
    public void StartSkillSelect()
    {
        Time.timeScale = 0.5f;

    }

    public void EndSkillSelect()
    {
        Time.timeScale = 1f;

    }

    public void ShaderChange()
    {
        //string shaderName = "Mobile/Unlit (Supports Lightmap)";

        //GameObject sampleObject;


        //MeshRenderer ren = sampleObject.GetComponent<MeshRenderer>();   // �ϴ� MeshRenderer ������Ʈ�� ���

        //ren.material.shader = Shader.Find(shaderName);                  // ���̴��� ã��(�̸�����) ����
    }

    public void ShaderMetChange()
    {
        //void TestOnOff()
        //{
        //    var mt = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        //    switch (keyValue)
        //    {
        //        case 1:
        //            mt.EnableKeyword("_USEGRAY_ON");
        //            mt.DisableKeyword("_USEGRAY_OFF");
        //            mt.DisableKeyword("_USEGRAY_BLACK");
        //            break;
        //        case 2:
        //            mt.DisableKeyword("_USEGRAY_ON");
        //            mt.EnableKeyword("_USEGRAY_OFF");
        //            mt.DisableKeyword("_USEGRAY_BLACK");
        //            break;
        //        case 3:
        //            mt.DisableKeyword("_USEGRAY_ON");
        //            mt.DisableKeyword("_USEGRAY_OFF");
        //            mt.EnableKeyword("_USEGRAY_BLACK");
        //            break;
        //    }
        //}
        //void TestToggle()
        //{
        //    isGray = !isGray;
        //    float value = isGray ? 1f : 0f;
        //    Debug.Log(value);
        //    var mt = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        //    mt.SetFloat("GRAY", value);
        //}
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
