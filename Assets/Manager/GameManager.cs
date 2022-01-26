using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Events;

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

    // Vars
    public GameObject gameoverUI;
    private GameState state;

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
}
