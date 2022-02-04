using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private const float cameraMovetime = 0.7f;



    // 어둠 / 슬로우 관련 함수 =====================================================================
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


        //MeshRenderer ren = sampleObject.GetComponent<MeshRenderer>();   // 일단 MeshRenderer 컴포넌트를 얻고

        //ren.material.shader = Shader.Find(shaderName);                  // 쉐이더를 찾아(이름으로) 변경
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
