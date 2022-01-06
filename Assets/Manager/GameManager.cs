using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gm;
    private MultiTouch mt;
    private SaveLoadManager sm;

    public static GameManager Manager => gm;
    public MultiTouch MultiTouch => mt;
    public SaveLoadManager SaveLoad => sm;

    private void Awake()
    {
        gm = this;

        mt = MultiTouch.Instance;
        sm = SaveLoadManager.Instance;
    }
}
