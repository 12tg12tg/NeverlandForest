using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    None, Battle, Hunt, Gathering, Cook, Camp, Dungeon, Tutorial
}

public enum GameOverType
{
    BattleLoss,
    StaminaZero,
    WitchCaught,
}

public enum GameScene
{
    World, Dungeon, Hunt, Battle, Camp, TutorialDungeon
}

[DefaultExecutionOrder(-2)]
public class GameManager : Singleton<GameManager> // 타이틀 화면에서 생성
{
    // Singleton
    private SaveLoadManager sm;
    private MultiTouch mt;

    // Managers
    private static GameManager gm;
    private WorldMapManager wm;
    private CameraManager cm;
    [SerializeField] private TutorialManager tm;
    [SerializeField] private Production pd;
    [SerializeField] private StoryManager stm;

    // Vars
    private GameState state;

    [Header("UI 연결")]
    [SerializeField] private GameObject gameoverUI;

    [Header("UI 연출용 카메라")]
    [SerializeField] private ScreenFixed productionCamera;
    public ScreenFixed ProductionCamera { get => productionCamera; set => productionCamera = value; }

    private void Awake() // 게임 실행시 준비
    {
        SingletonInit();

        #region 카메라 안쓰고있음
        //if (CamManager != null)
        //{
        //    if (CamManager.worldMapCamera != null)
        //    {
        //        var wmCamera = CamManager.worldMapCamera.GetComponent<WorldMapCamera>();
        //        if (wmCamera != null)
        //            wmCamera.Init();
        //    }

        //    if (CamManager.miniWorldMapCamera != null)
        //    {
        //        var wmmCamera = CamManager.miniWorldMapCamera.GetComponent<WorldMapCamera>();
        //        if (wmmCamera != null)
        //        {
        //            wmmCamera.Init();
        //        }
        //    }
        //}
        #endregion // 카메라 잠구기

        // 로드
        LoadAllSavedata();

        //if (WorldManager != null)
        //    WorldManager.Init();

        // 로드된 데이터를 기반으로 추가 전역 데이터 설정
        ConsumeManager.Init();

        // 오브젝트풀 생성
        ObjectPoolInit();
    }

    // 초기화 ======================================================================
    private void SingletonInit()
    {
        gm = Instance;
        mt = MultiTouch.Instance;
        sm = SaveLoadManager.Instance;
        DontDestroyOnLoad(this);
    }
    private void LoadAllSavedata()
    {
        int count = (int)SaveLoadSystem.SaveType.Count;
        for (int i = 0; i < count; i++)
        {
            sm.Load((SaveLoadSystem.SaveType)i);
        }
    }
    private void ObjectPoolInit()
    {
        MonsterPool.Instance.Init();
        ProjectilePool.Instance.Init();
        UIPool.Instance.Init();
        TrapPool.Instance.Init();
    }

    // 씬 호출 ======================================================================
    public void LoadScene(GameScene scene)
    {
        ReleaseValue();
        switch (scene)
        {
            case GameScene.World:
                SceneManager.LoadScene("AS_WorldMap");
                break;
            case GameScene.Dungeon:
                SceneManager.LoadScene("AS_RandomMap");
                break;
            case GameScene.Hunt:
                SceneManager.LoadScene("AS_Hunting");
                break;
            case GameScene.Battle:
                SceneManager.LoadScene("JYK_Test_Battle");
                break;
            case GameScene.Camp:
                SceneManager.LoadScene("JYK_Test_Main");
                break;
            case GameScene.TutorialDungeon:
                SceneManager.LoadScene("2ENO_TutorialDungeon");
                break;
        }
    }
    private void ReleaseValue()
    {
        wm = null;
        cm = null;
    }

    // 게임오버 ======================================================================
    public void GameOver(GameOverType type)
    {
        gameoverUI.SetActive(true);
        var texts = gameoverUI.GetComponentsInChildren<TMP_Text>();
        var text = texts.Where(x => x.CompareTag("GameOverText")).Select(x => x).FirstOrDefault();
        switch (type)
        {
            case GameOverType.BattleLoss:
                text.text = "몬스터의 습격으로부터 몸을 지키지 못했다." + "\n" + "\"엄마..!\"";
                break;
            case GameOverType.StaminaZero:
                text.text = "잠과 식사를 제대로 해결하지 못한 탓에 끝내 쓰러지고 말았다." + "\n" + "\"더 이상은.. 무리야....\"";
                break;
            case GameOverType.WitchCaught:
                text.text = "마녀에게 붙잡혀 발버둥치는 내용" + "\n" + "\"살려주세요....!\"";
                break;
        }
    }

    public void DungeonGiveUp()
    {
        // TODO : 던전 포기 기능은 던전 씬에서만 동작 하도록 추가 해야함
        Vars.UserData.WorldMapPlayerData.isClear = false;
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoToTitle() // 죽었을 때 타이틀로 버튼 클릭 함수
    {
        pd.FadeIn(() => {
            // TODO : 사망 했을 때 지워져야 할 세이브 데이터들 및 초기화 되어야 하는 유저 데이터들 여기에
            var len = System.Enum.GetValues(typeof(SaveDataName)).Length;
            for (int i = 0; i < len; i++)
            {
                Utility.DeleteSaveData((SaveDataName)i);
            }
            Vars.UserData.WorldMapNodeStruct = new List<WorldMapNodeStruct>();
            Vars.UserData.WorldMapPlayerData = default;
            Vars.UserData.uData.Date = 0;
            // End
            SceneManager.LoadScene("Game");
        });
    }
    public void GoToGameEnd() // 버튼 클릭 함수 (옵션창, 게임오버창)에서 사용해야함
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Property ======================================================================
    public CameraManager CamManager
    {
        get
        {
            if (cm == null)
            {
                var go = (CameraManager)FindObjectOfType(typeof(CameraManager));
                cm = go;
            }
            return cm;
        }
    }
    public WorldMapManager WorldManager
    {
        get
        {
            if (wm == null)
            {
                var go = (WorldMapManager)FindObjectOfType(typeof(WorldMapManager));
                wm = go;
            }
            return wm;
        }
    }
    public TutorialManager TutoManager
    {
        get
        {
            return tm;
        }
    }
    public GameState State
    {
        get => state;
        set => this.state = value;
    }
    public static GameManager Manager => gm;
    public MultiTouch MultiTouch => mt;
    public SaveLoadManager SaveLoad => sm;
    public Production Production => pd;
    public StoryManager StoryManager => stm;

}