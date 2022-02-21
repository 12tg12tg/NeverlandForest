using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleTutorial : MonoBehaviour
{
    //===========================================================================
    // 튜토리얼 변수 영역
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;
    private BottomUIManager bottomUI;

    [Header("마스크 재료 설정")]
    [SerializeField] private Sprite rect;
    [SerializeField] private Sprite circle;

    [Header("마스크 & 배경")]
    [SerializeField] private GameObject mastGo;
    [SerializeField] private RectTransform maskRt;
    [SerializeField] private Image maskImg;
    [SerializeField] private GameObject background;
    [SerializeField] private RectTransform fingerImg;
    [SerializeField] private RectTransform dialogBoxRt;
    [SerializeField] private DialogBoxObject dialogBox;

    public bool tu_01_CamRightButton;
    public bool tu_02_CamLeftButton;
    public bool tu_03_TrapClick1;
    public bool tu_03_TrapClick2;
    public bool tu_04_TileClick;
    public bool tu_05_BattleStart;
    public bool tu_07_BoySkill1;
    public bool tu_07_BoySkill2;
    public bool tu_10_GirlSkill1;
    public bool tu_10_GirlSkill2;
    public bool tu_12_GirlSkill1;
    public bool tu_12_GirlSkill2;
    public bool isWin;
    public bool isLose;

    private bool isWaitingTouch;
    private bool isTouched;
    public bool lockSkillButtonClick;
    public bool lockSkillButtonDrag;
    public bool lockTileClick;
    public bool lockAutoBattleStateChange;
    private void Init()
    {
        bottomUI = BottomUIManager.Instance;
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
        lockSkillButtonClick = true;
        lockTileClick = true;
        lockAutoBattleStateChange = true;
        lockSkillButtonDrag = true;

        // 스토리에 쓰는중
        stm = GameManager.Manager.StoryManager;
        boy = BattleManager.Instance.boy;
        girl = BattleManager.Instance.girl;
    }
    //===========================================================================
    // 스토리 변수 영역
    private StoryManager stm;
    private PlayerBattleController boy;
    private PlayerBattleController girl;
    private bool storyChapter1to2 = false;
    private bool storyChapter3 = false;


    //===========================================================================
    public void StartDutorial()
    {
        bm.uiLink.option.interactable = false;
        gameObject.SetActive(true);
        Init();
        StartCoroutine(CoBattleTutorial());
    }
    public void EndDutorial()
    {
        // 인벤토리 아이템 추가
        RewardItem();

        bm.uiLink.AllButtonInteractive(true);
        GameManager.Manager.TutoManager.mainTutorial.NextMainTutorial();
    }

    private void RewardItem()
    {
        // 인벤토리 비우기
        DataAllItem temp;
        var tempInventory = new List<DataAllItem>(Vars.UserData.HaveAllItemList);
        foreach (var item in tempInventory)
        {
            temp = new DataAllItem(item);
            Vars.UserData.RemoveItemData(temp);
        }

        // 화살
        var allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_20"));
        temp.OwnCount = 20;
        Vars.UserData.AddItemData(temp); // 100발

        // 오일
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_19"));
        temp.OwnCount = 8;
        Vars.UserData.AddItemData(temp); // 8개

        // 나무도막
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_1"));
        temp.OwnCount = 5;
        Vars.UserData.AddItemData(temp); // 5개

        // 올가미
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_14"));
        temp.OwnCount = 5;
        Vars.UserData.AddItemData(temp); // 5개

        // 부비트랩
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_15"));
        temp.OwnCount = 5;
        Vars.UserData.AddItemData(temp); // 5개

        // 나무트랩
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_16"));
        temp.OwnCount = 5;
        Vars.UserData.AddItemData(temp); // 5개

        // 가시트랩
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_17"));
        temp.OwnCount = 5;
        Vars.UserData.AddItemData(temp); // 5개

        // 삽
        allItemTable = DataTableManager.GetTable<AllItemDataTable>();
        temp = new DataAllItem(allItemTable.GetData<AllItemTableElem>("ITEM_13"));
        temp.OwnCount = 1;
        Vars.UserData.AddItemData(temp); // 5개

        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.item);
    }

    private void Update()
    {
        if (isWaitingTouch)
        {
            if (GameManager.Manager.MultiTouch.IsTap)
            {
                isWaitingTouch = false;
                isTouched = true;
            }
        }
        if (GameManager.Manager.MultiTouch.IsTap && (storyChapter1to2 || storyChapter3))
        {
            stm.isNext = true;
        }
    }

    //===========================================================================
    private IEnumerator CoBattleStoryStart()
    {
        storyChapter1to2 = true;
        #region 챕터 1
        // 챕터1 준비) 타일베이스 게임오브젝트 안보이고, 카메라 및 플레이어들 위치 지정
        TileMaker.Instance.gameObject.SetActive(false);
        GameManager.Manager.CamManager.uiCamera.gameObject.SetActive(false);
        var cameraTrans = Camera.main.transform;
        var op = cameraTrans.localPosition;
        var or = cameraTrans.localRotation;
        cameraTrans.localPosition = new Vector3(0f, 2.9f, -4.6f);
        cameraTrans.localRotation = Quaternion.Euler(new Vector3(21.153f, 0f, 0f));
        boy.transform.localPosition = new Vector3(0f, 0f, 1.5903f);

        // 챕터1
        var isNextChapter = false;
        stm.MessageBox.SetActive(true); // 대화창 오픈
        StartCoroutine(stm.CoStory(StoryType.Chapter1, () => isNextChapter = true));
        yield return new WaitWhile(() => !isNextChapter);
        #endregion
        #region 챕터 2
        // 챕터2 준비) 카메라 및 플레이어들 위치 재 정의
        girl.transform.localPosition = new Vector3(5f, 0f, 1.59f);
        girl.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        cameraTrans.localPosition += new Vector3(2.5f, 0f, 0f);

        // 챕터2
        isNextChapter = false;
        StartCoroutine(stm.CoStory(StoryType.Chapter2, () => isNextChapter = true));
        yield return new WaitWhile(() => !isNextChapter);
        #endregion
        
        // 준비 취소) 타일베이스 게임오브젝트 보이게
        storyChapter1to2 = false;
        girl.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        cameraTrans.localPosition = op;
        cameraTrans.localRotation = or;
        stm.MessageBox.SetActive(false);
        TileMaker.Instance.gameObject.SetActive(true);
        GameManager.Manager.CamManager.uiCamera.gameObject.SetActive(true);
    }

    private IEnumerator CoBattleStoryEnd()
    {
        storyChapter3 = true;
        #region 챕터 3
        // 챕터3 준비) 타일베이스 게임오브젝트 안보이고, 카메라 및 플레이어들 위치 지정
        TileMaker.Instance.gameObject.SetActive(false);
        GameManager.Manager.CamManager.uiCamera.gameObject.SetActive(false);
        var cameraTrans = Camera.main.transform;
        cameraTrans.localPosition = new Vector3(2.5f, 2.9f, -4.6f);
        cameraTrans.localRotation = Quaternion.Euler(new Vector3(21.153f, 0f, 0f));
        boy.PlayIdleAnimation();
        girl.PlayIdleAnimation();
        boy.transform.localPosition = new Vector3(0f, 0f, 1.5903f);
        girl.transform.localPosition = new Vector3(5f, 0f, 1.59f);
        girl.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));

        // 챕터3
        var isNextChapter = false;
        stm.MessageBox.SetActive(true); // 대화창 오픈
        StartCoroutine(stm.CoStory(StoryType.Chapter3, () => isNextChapter = true));
        yield return new WaitWhile(() => !isNextChapter);
        #endregion

        // 되돌리기
        storyChapter1to2 = false;
        stm.MessageBox.SetActive(false);
        TileMaker.Instance.gameObject.SetActive(true);
        GameManager.Manager.CamManager.uiCamera.gameObject.SetActive(true);
    }


    private IEnumerator CoBattleTutorial()
    {
        // 스토리
        yield return StartCoroutine(CoBattleStoryStart());
        GameManager.Manager.Production.FadeOut();
        // 웨이브 생성
        bm.TutorialInit();
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(3f);

        // 입력 1. 웨이브 미리보기 버튼 클릭 유도 & 설명 띄우기
        bm.uiLink.moveCameraRightButton.interactable = true;
        UISet_1st_Enter();
        yield return new WaitUntil(() => tu_01_CamRightButton);
        UISet_1st_End();
        yield return new WaitForSeconds(1.5f);

        // 입력 2. 웨이브 미리보기 취소 클릭 유도 & 설명 띄우기
        bm.uiLink.moveCameraLeftButton.interactable = true;
        UISet_2nd_Enter();
        yield return new WaitUntil(() => tu_02_CamLeftButton);
        UISet_2nd_End();
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(1.5f);

        // 입력 3. 인벤토리 나무트랩 클릭 유도 & 설명 띄우기
        bottomUI.ButtonInteractive(false);
        var targetButton = bottomUI.itemButtons[2].GetComponent<Button>();
        targetButton.interactable = true;
        UISet_3rd_01_Enter();
        yield return new WaitUntil(() => tu_03_TrapClick1);
        targetButton.interactable = false;

        var popupButtons = bottomUI.popUpWindow.GetComponentsInChildren<Button>();
        popupButtons[0].interactable = true;
        popupButtons[1].interactable = false;
        UISet_3rd_02_Enter();
        yield return new WaitUntil(() => tu_03_TrapClick2);
        popupButtons[0].interactable = true;
        popupButtons[1].interactable = true;
        UISet_3rd_End();


        // 입력 4. 특정 타일선택 클릭
        UISet_4th_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_04_TileClick);
        lockTileClick = true;
        UISet_4th_End();
        yield return new WaitForSeconds(1.5f);

        // 입력 5. 전투시작버튼 터치 & 설명
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        targetButton = bm.uiLink.battleStartButton;
        targetButton.interactable = true;
        UISet_5th_Enter();
        yield return new WaitUntil(() => tu_05_BattleStart);
        UISet_5th_End();
        yield return new WaitForSeconds(1f);
        bottomUI.ButtonInteractive(false);
        bm.uiLink.AllButtonInteractive(false);
        yield return new WaitForSeconds(2.2f);


        // 입력 6. 설명(사냥꾼 능력)
        bottomUI.SkillButtonInit();
        UISet_6th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_6th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_6th_End();
        yield return new WaitForSeconds(0.5f);


        // 입력 7. 사냥꾼 스킬 사용 유도
        UISet_7th_01_Enter();
        lockSkillButtonClick = false;
        bottomUI.skillButtons[2].ownButton.interactable = true;
        yield return new WaitUntil(() => tu_07_BoySkill1);

        lockSkillButtonClick = true;
        bottomUI.skillButtons[2].ownButton.interactable = false;

        UISet_7th_02_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_07_BoySkill2);
        UISet_7th_End();
        lockTileClick = true;
        yield return new WaitForSeconds(3.5f);

        // 입력 8. 몬스터 실드 설명
        UISet_8th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_8th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_8th_End();
        yield return new WaitForSeconds(0.7f);

        // 입력 9. 설명(약초학자 능력)
        UISet_9th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_9th_End();
        yield return new WaitForSeconds(0.5f);

        // 입력 10. 약초학자 스킬 사용 유도
        UISet_10th_01_Enter();
        lockSkillButtonClick = false;
        bottomUI.skillButtons[8].ownButton.interactable = true;
        yield return new WaitUntil(() => tu_10_GirlSkill1);
        lockSkillButtonClick = true;
        bottomUI.skillButtons[8].ownButton.interactable = false;

        UISet_10th_02_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_10_GirlSkill2);
        UISet_10th_End();
        lockTileClick = true;
        yield return new WaitForSeconds(2f);

        // 입력 11. 랜턴 밝기 감소 설명
        UISet_11th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_11th_02_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_11th_03_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_11th_End();

        // ... 몬스터 행동(설명따로없음)
        bm.FSM.ChangeState(BattleState.Monster);
        bm.uiLink.turnSkipTrans.SetActive(false);
        lockAutoBattleStateChange = false;
        yield return new WaitForSeconds(6.5f);

        // 입력 12. 랜턴 충전 유도
        UISet_12th_01_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;

        UISet_12th_02_Enter();
        bottomUI.skillButtons[11].ownButton.interactable = true;
        lockSkillButtonClick = false;
        yield return new WaitUntil(() => tu_12_GirlSkill1);
        lockSkillButtonClick = true;
        bottomUI.skillButtons[11].ownButton.interactable = false;

        UISet_12th_03_Enter();
        lockTileClick = false;
        yield return new WaitUntil(() => tu_12_GirlSkill2);
        lockTileClick = true;
        UISet_12th_End();
        yield return new WaitForSeconds(3.5f);

        // 입력 13. 랜턴 충전 확인 클릭
        UISet_13th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_13th_End();

        // 입력 14. 남은 전투 유저가 하기(설명문)
        UISet_14th_Enter();
        isWaitingTouch = true;
        yield return new WaitUntil(() => isTouched);
        isTouched = false;
        UISet_14th_End();

        lockTileClick = false;
        lockSkillButtonClick = false;
        bottomUI.ButtonInteractive(true);
        bm.uiLink.AllButtonInteractive(true);
        bottomUI.UpdateSkillInteractive();
        lockAutoBattleStateChange = false;
        lockSkillButtonDrag = false;

        while (!isWin)
        {
            yield return new WaitUntil(() => isWin || isLose); /*승리 ? 패배? 기다리기*/

            if (isWin)
                break;

            // 배틀상태멈추기
            bm.FSM.ChangeState(BattleState.End);


            // 메세지창 및 재설정
            isLose = false;
            UISet_15th_Enter();
            lockAutoBattleStateChange = true;
            lockSkillButtonDrag = true;
            lockTileClick = true;
            lockSkillButtonClick = true;

            bottomUI.ButtonInteractive(false);
            bm.uiLink.AllButtonInteractive(false);
            bottomUI.UpdateSkillInteractive();

            isWaitingTouch = true;
            yield return new WaitUntil(() => isTouched);
            isTouched = false;
            UISet_15th_End();

            lockTileClick = false;
            lockSkillButtonClick = false;
            lockAutoBattleStateChange = false;
            lockSkillButtonDrag = false;
            bottomUI.ButtonInteractive(true);
            bm.uiLink.AllButtonInteractive(true);
            bottomUI.UpdateSkillInteractive();

            RestartBattle();
        }

        // 스토리 챕터3
        GameManager.Manager.Production.FadeOut();
        yield return StartCoroutine(CoBattleStoryEnd());

        //승리 - 종료 (보상창을 띄울것인지, 씬은 어디로 넘어갈 것인지)
        EndDutorial();
    }

    // ====================================================================================
    // ====================================================================================
    // ====================================================================================
    // ====================================================================================
    #region 대기함수
    private void UISet_1st_Enter() // 웨이브 미리보기 버튼 클릭 유도 & 설명 띄우기
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(534f, -0.01f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(805, 300);
        dialogBox.text.text = "이 버튼을 눌러서 몬스터 웨이브를 미리 살펴봅니다.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(1164, 285);
    }
    private void UISet_1st_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_2nd_Enter() // 웨이브 미리보기 취소 클릭 유도 & 설명 띄우기
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-533f, -0.01f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(214, 300);
        dialogBox.text.text = "몬스터가 고작 두마리 뿐이군요?\n다시 돌아가서 전투를 준비합시다!";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(126, 275);
    }
    private void UISet_2nd_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_3rd_01_Enter() // 인벤토리 나무트랩 클릭 유도 & 설명 띄우기
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(224.67f, -198f);
        maskRt.sizeDelta = new Vector2(65f, 65f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(733f, 335f);
        dialogBox.text.text = "설치물을 설치해서 전투에 도움을 받을 수 있습니다.\n나무트랩을 설치해봅시다!";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(863f, 94f);
    }
    private void UISet_3rd_02_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, -60f);
        maskRt.sizeDelta = new Vector2(91f, 36f);
        maskImg.sprite = rect;

        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(882.3f, 233f);
    }
    private void UISet_3rd_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_4th_Enter() // (특정) 타일선택 클릭
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, 24f);
        maskRt.sizeDelta = new Vector2(147f, 84f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(478f, 324f);
        dialogBox.text.text = "아까 확인한 웨이브 정보를 바탕으로\n중앙라인에 트랩을 설치해둡시다.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(887f, 317f);
    } 
    private void UISet_4th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_5th_Enter() // 전투시작버튼 터치 & 설명
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(0f, 159f);
        maskRt.sizeDelta = new Vector2(197f, 50f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 336f);
        dialogBox.text.text = "이제 전투에 진입해볼까요?";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(714f, 466f);
    } 
    private void UISet_5th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_6th_01_Enter() // 설명(사냥꾼 능력)
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-313f, 95f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(447f, 378f);
        dialogBox.text.text = "사냥꾼은 화살을 쏴서 사거리 제한 없는 공격을 합니다.\n";

        // 손가락 위치
        fingerImg.gameObject.SetActive(false);
    }
    private void UISet_6th_02_Enter() // 설명(사냥꾼 능력)
    {
        dialogBox.text.text = "화살에 맞은 몬스터는 Hp 피해를 입으며 속박됩니다.\n속박에 걸린 턴은 움직이지 못합니다.";
    }
    private void UISet_6th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_7th_01_Enter() // 사냥꾼 스킬 사용 유도
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, -197f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(741, 228);
        dialogBox.text.text = "사냥꾼의 스킬을 사용해봅시다.\n버튼을 눌러 스킬을 활성화시키세요.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(876, 89);
    }
    private void UISet_7th_02_Enter() // 사냥꾼 스킬 사용 유도
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(334f, 36f);
        maskRt.sizeDelta = new Vector2(145f, 105f);
        maskImg.sprite = circle;

        // 대화상자 삭제
        dialogBoxRt.gameObject.SetActive(false);

        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(986f, 321f);
    }
    private void UISet_7th_End()
    {
        maskRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_8th_01_Enter() // 몬스터 실드 설명
    {
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(651f, 351f);
        dialogBox.text.text = "방금 보셨나요?\n 화살을 맞았는데 데미지가 0이라니..\n";
    }
    private void UISet_8th_02_Enter() // 몬스터 실드 설명
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(353f, 55f);
        maskRt.sizeDelta = new Vector2(31f, 5f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBox.text.text = "몬스터들은 물리공격을 방어하는 실드를 가지고 있습니다.\n때문에 사냥꾼의 공격은 실드수치만큼 감소된 데미지가 적용됩니다.";
    }
    private void UISet_8th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
    }

    private void UISet_9th_Enter() // 설명(약초학자 능력)
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-388f, 0f);
        maskRt.sizeDelta = new Vector2(123f, 123f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(371f, 300f);
        dialogBox.text.text = "약초학자의 랜턴공격으로 실드를 차감시킬 수 있습니다!";

        // 손가락 위치
        fingerImg.gameObject.SetActive(false);
    }
    private void UISet_9th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_10th_01_Enter() // 약초학자 스킬 사용 유도
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(225f, -261.8f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(741f, 159f);
        dialogBox.text.text = "약초학자의 스킬을 사용해봅시다.\n버튼을 눌러 스킬을 활성화시키세요.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(876f, 40f);
    }
    private void UISet_10th_02_Enter() // 사냥꾼 스킬 사용 유도
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(334f, 36f);
        maskRt.sizeDelta = new Vector2(145f, 105f);
        maskImg.sprite = circle;

        // 대화상자 삭제
        dialogBoxRt.gameObject.SetActive(false);

        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(986f, 321f);
    }
    private void UISet_10th_End()
    {
        maskRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_11th_01_Enter() // 랜턴 밝기 감소 설명
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(349f, 53f);
        maskRt.sizeDelta = new Vector2(39f, 12f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(636f, 363f);
        dialogBox.text.text = "약초학자의 데미지는 몬스터의 실드를 먼저 차감시키고\n남은 데미지는 Hp를 감소시킵니다.";
    }
    private void UISet_11th_02_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-11.4f, 248f);
        maskRt.sizeDelta = new Vector2(325f, 92f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(506f, 395f);
        dialogBox.text.text = "약초학자의 스킬은 강력하지만, 랜턴 밝기를 소모합니다.\n남은 랜턴밝기는 스킬의 범위와 데미지에 영향을 줍니다.";
    }
    private void UISet_11th_03_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(441f, -105f);
        maskRt.sizeDelta = new Vector2(75f, 75f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(true);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(749f, 193f);
        dialogBox.text.text = "약초학자 스킬의 최대범위는 이 아이콘이 표시된 줄까지 입니다.";
    }
    private void UISet_11th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_12th_01_Enter() // 랜턴 충전 스킬 사용
    {
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "약초학자에게는 랜턴을 충전하는 스킬이 있습니다.";
    }
    private void UISet_12th_02_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(426f, -262f);
        maskRt.sizeDelta = new Vector2(61f, 59f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(true);
        dialogBoxRt.anchoredPosition = new Vector2(940f, 186f);
        dialogBox.text.text = "터치하여 \"오일 충전\" 스킬을 활성화하세요.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(1080f, 47f);
    }
    private void UISet_12th_03_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-375f, -12f);
        maskRt.sizeDelta = new Vector2(134f, 134f);
        maskImg.sprite = circle;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(true);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(368f, 284f);
        dialogBox.text.text = "약초학자를 클릭하여 랜턴을 충전합니다.";
        // 손가락 위치
        fingerImg.gameObject.SetActive(true);
        fingerImg.anchoredPosition = new Vector2(261f, 278f);
    }
    private void UISet_12th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_13th_Enter()
    {
        // 마스크 위치 & 크기 & 도형
        maskRt.gameObject.SetActive(true);
        maskRt.anchoredPosition = new Vector2(-11.4f, 248f);
        maskRt.sizeDelta = new Vector2(325f, 92f);
        maskImg.sprite = rect;
        // 설명박스 위치 & 내용 & 화살표
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(true);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(506f, 395f);
        dialogBox.text.text = "오일이 소모되어 랜턴 게이지가 충전되었습니다.";
    }
    private void UISet_13th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_14th_Enter()
    {
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "다양한 스킬을 활용하여 전투에서 승리하세요!";
    }
    private void UISet_14th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }

    private void UISet_15th_Enter()
    {
        dialogBoxRt.gameObject.SetActive(true);
        dialogBox.up.SetActive(false);
        dialogBox.left.SetActive(false);
        dialogBox.right.SetActive(false);
        dialogBox.down.SetActive(false);
        dialogBoxRt.anchoredPosition = new Vector2(508f, 300f);
        dialogBox.text.text = "튜토리얼을 성의있게 진행해주세요!";
    }
    private void UISet_15th_End()
    {
        maskRt.gameObject.SetActive(false);
        dialogBoxRt.gameObject.SetActive(false);
        fingerImg.gameObject.SetActive(false);
    }


    private void RestartBattle()
    {
        var list = new List<MonsterUnit>(bm.monsters);
        list.AddRange(bm.waveLink.wave1);
        list.AddRange(bm.waveLink.wave2);
        list.AddRange(bm.waveLink.wave3);
        bm.waveLink.wave1.Clear();
        bm.waveLink.wave2.Clear();
        bm.waveLink.wave3.Clear();
        tm.TileClear();
        list.ToList().ForEach(n => { if (n != null) n.Release(); });

        bm.TutorialInit(true);
    }
    #endregion
}
