using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private MultiTouch mt;
    private SaveLoadManager sm;
    public MultiTouch MultiTouch => mt;
    public SaveLoadManager SaveLoad => sm;

    private void Awake()
    {
        mt = MultiTouch.Instance;
        sm = SaveLoadManager.Instance;
    }
}
