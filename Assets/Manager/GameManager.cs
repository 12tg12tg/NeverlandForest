using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None, Battle, Hunt, Gathering, Cook, 
}


public class GameManager : MonoBehaviour
{
    private static GameManager gm;
    private MultiTouch mt;
    private SaveLoadManager sm;

    private GameState state;

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

}
