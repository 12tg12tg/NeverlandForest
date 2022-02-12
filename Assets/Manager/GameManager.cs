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
public class GameManager : Singleton<GameManager> // Ÿ��Ʋ ȭ�鿡�� ����
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
    public ContentsRewards reward = new ContentsRewards();

    [Header("UI ����")]
    [SerializeField] private GameObject gameoverUI;

    [Header("UI ����� ī�޶�")]
    [SerializeField] private ScreenFixed productionCamera;
    public ScreenFixed ProductionCamera { get => productionCamera; set => productionCamera = value; }
    private void Awake() // ���� ����� �غ�
    {
        gm = Instance;

        if (FindObjectsOfType(typeof(GameManager)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
        }


        SingletonInit();

        #region ī�޶� �Ⱦ�������
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
        #endregion // ī�޶� �ᱸ��

        // �ε�
        LoadAllSavedata();

        // �ε�� �����͸� ������� �߰� ���� ������ ����
        ConsumeManager.Init();

        // ������ƮǮ ����
        ObjectPoolInit();
    }

    // �ʱ�ȭ ======================================================================
    private void SingletonInit()
    {
        mt = MultiTouch.Instance;
        sm = SaveLoadManager.Instance;

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

    // �� ȣ�� ======================================================================
    public void LoadScene(GameScene scene)
    {
        SoundManager.Instance.PlayWalkSound(false);
        //�� ����ǵ� �Ȳ���
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

    // ���ӿ��� ======================================================================
    public void GameOver(GameOverType type)
    {
        gameoverUI.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_GameOver);

        var texts = gameoverUI.GetComponentsInChildren<TMP_Text>();
        var text = texts.Where(x => x.CompareTag("GameOverText")).Select(x => x).FirstOrDefault();
        switch (type)
        {
            case GameOverType.BattleLoss:
                text.text = "������ �������κ��� ���� ��Ű�� ���ߴ�." + "\n" + "\"����..!\"";
                break;
            case GameOverType.StaminaZero:
                text.text = "��� �Ļ縦 ����� �ذ����� ���� ſ�� ���� �������� ���Ҵ�." + "\n" + "\"�� �̻���.. ������....\"";
                break;
            case GameOverType.WitchCaught:
                text.text = "���࿡�� ������ �߹���ġ�� ����" + "\n" + "\"����ּ���....!\"";
                break;
        }
    }

    public void DungeonGiveUp()
    {
        // TODO : ���� ���� ����� ���� �������� ���� �ϵ��� �߰� �ؾ���
        Vars.UserData.WorldMapPlayerData.isClear = false;
        SceneManager.LoadScene("AS_WorldMap");
    }
    public void GoToTitle() // �׾��� �� Ÿ��Ʋ�� ��ư Ŭ�� �Լ�
    {
        pd.FadeIn(() => {
            // TODO : ��� ���� �� �������� �� ���̺� �����͵� �� �ʱ�ȭ �Ǿ�� �ϴ� ���� �����͵� ���⿡
            var len = System.Enum.GetValues(typeof(SaveDataName)).Length;
            for (int i = 0; i < len; i++)
            {
                Utility.DeleteSaveData((SaveDataName)i);
            }
            //���������;ȿ� �Լ� ����� �ѹ� ȣ���ؼ� �ʱ�ȭ�ϴ� ������� ����.
            Vars.UserData.UserDataInit();
            WorldMapCamera.isInit = false; // ����� ī�޶� �ʱ�ȭ
            SceneManager.LoadScene("Game");
        });
    }
    public void GoToGameEnd() // ��ư Ŭ�� �Լ� (�ɼ�â, ���ӿ���â)���� ����ؾ���
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