using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MainManager mainMgr;
    void Start()
    {
        mainMgr = MainManager.Instance;
        mainMgr.Init();

        //SaveLoadSystem.Init();
    }
}
