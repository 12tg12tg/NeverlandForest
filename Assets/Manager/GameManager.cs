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
    [Header("월드맵")]
    public WorldMapManager wm;
    [Header("카메라")]
    public CameraManager cm;
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
        if (wm != null)
            wm.Init();

        DontDestroyOnLoad(this);
        //MonsterPool.Instance.Init();
    }

    public void GameOver(GameOverType type)
    {
        gameoverUI.SetActive(true);
        var texts = gameoverUI.GetComponentsInChildren<TMP_Text>();
        var text = texts.Where(x => x.CompareTag("GameOverText")).Select(x => x).FirstOrDefault();

        switch (type)
        {
            case global::GameOverType.BattleLoss:
                text.text = "몬스터의 습격으로부터 몸을 지키지 못했다." + "\n" + "\"엄마..!\"";
                break;
            case global::GameOverType.StaminaZero:
                text.text = "잠과 식사를 제대로 해결하지 못한 탓에 끝내 쓰러지고 말았다." + "\n" + "\"더 이상은.. 무리야....\"";
                break;
            case global::GameOverType.WitchCaught:
                text.text = "마녀에게 붙잡혀 발버둥치는 내용" + "\n" + "\"살려주세요....!\"";
                break;
        }
    }

    public void GoToTitle() // 타이틀씬이 없어서 현재씬을 재시작 하는 형태로 되어있음
    {
        var coScene = Utility.CoSceneChange(SceneManager.GetActiveScene().buildIndex, 1f, () =>
        {
            Utility.DeleteSaveData(SaveDataName.TextWorldMapPlayerDataPath);
            Utility.DeleteSaveData(SaveDataName.TextWorldMapDataPath);
            Debug.Log("사망");
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