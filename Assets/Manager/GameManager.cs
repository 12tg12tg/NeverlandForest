using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    None, Battle, Hunt, Gathering, Cook, Camp, Dungeon
}

public enum GameOverType
{
    BattleLoss,
    StaminaZero,
    WitchCaught,
}

public class GameManager : MonoBehaviour
{
    // Managers
    private static GameManager gm;
    private SaveLoadManager sm;
    private MultiTouch mt;
    [Header("�����")]
    public WorldMapManager wm;
    [Header("ī�޶�")]
    public CameraManager cm;
    [Header("Ʃ�丮��")]
    public TutorialManager tm;
    // Vars
    private GameState state;
    [Header("UI")]
    public GameObject gameoverUI;
    

    // Property
    public GameState State
    {
        get => state;
        set => this.state = value;
    }
    public static GameManager Manager => gm;
    public MultiTouch MultiTouch => mt;
    public SaveLoadManager SaveLoad => sm;

    private void Awake()
    {
        gm = this;
        mt = MultiTouch.Instance;
        sm = SaveLoadManager.Instance;

        // TODO : �ش� �κ� ���� �ʿ�
        if(cm != null)
        {
            if (cm.mainCamera != null)
            {
                var wmCamera = cm.mainCamera.GetComponent<WorldMapCamera>();
                if (wmCamera != null)
                    wmCamera.Init();
            }

            if (cm.miniWorldMapCamera != null)
            {
                var wmmCamera = cm.miniWorldMapCamera.GetComponent<WorldMapCamera>();
                if (wmmCamera != null)
                {
                    wmmCamera.Init();
                }
            }
        }
        if (wm != null)
            wm.Init();

        // �ε�
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.Battle);


        DontDestroyOnLoad(this);
        ConsumeManager.init();
        //MonsterPool.Instance.Init();
    }

    public void GameOver(GameOverType type)
    {
        gameoverUI.SetActive(true);
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

    public void GoToTitle() // Ÿ��Ʋ���� ��� ������� ����� �ϴ� ���·� �Ǿ�����
    {
        var coScene = Utility.CoSceneChange(SceneManager.GetActiveScene().buildIndex, 1f, () =>
        {
            Utility.DeleteSaveData(SaveDataName.TextWorldMapPlayerDataPath);
            Utility.DeleteSaveData(SaveDataName.TextWorldMapDataPath);
            Debug.Log("���");
            Vars.UserData.WorldMapNodeStruct = new List<WorldMapNodeStruct>();
            Vars.UserData.WorldMapPlayerData = default;
            Vars.UserData.uData.Date = 0;
        });
        StartCoroutine(coScene);
    }

    public void GoToGameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}