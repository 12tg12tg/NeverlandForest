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
}
