using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleOnGUI : MonoBehaviour
{
    private BattleManager manager;
    private void Start()
    {
        manager = BattleManager.Instance;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("블루문X, 마지막전투X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            manager.Init(false);
        }
        if (GUILayout.Button("블루문X, 마지막전투O", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            manager.Init(false, true);
        }


        if (GUILayout.Button("Lantern -12", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.ConsumeLantern(12);
        }
        if (GUILayout.Button("Lantern +12", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.FullingLantern(12);
        }


        if (GUI.Button(new Rect(Screen.width - 300, 0, 100, 50), "전투 종료"))
        {
            var list = new List<MonsterUnit>(manager.monsters);
            list.AddRange(manager.waveLink.wave1);
            list.AddRange(manager.waveLink.wave2);
            list.AddRange(manager.waveLink.wave3);
            manager.waveLink.wave1.Clear();
            manager.waveLink.wave2.Clear();
            manager.waveLink.wave3.Clear();
            list.ToList().ForEach(n => { if (n != null) n.Release(); });
            TileMaker.Instance.TileClear();
            manager.FSM.ChangeState(BattleState.Monster);
            manager.boy.PlayIdleAnimation();
            manager.girl.PlayIdleAnimation();
            manager.directingLink.HoldLantern();
        }
        if (GUI.Button(new Rect(Screen.width - 300, 50, 100, 50), "전투 종료 씬 전환"))
        {
            manager.monsters.ForEach(n => n.Release());
            GameManager.Manager.LoadScene(GameScene.Dungeon);
        }
        if (GUI.Button(new Rect(Screen.width - 300, 200, 100, 50), "튜토리얼"))
        {
            manager.tutorial.StartDutorial();
        }

        if (GUI.Button(new Rect(Screen.width - 300, 300, 100, 50), "블루문준비"))
        {
            BattleManager.initState = BattleInitState.BluemoonSet;
            manager.Release();
            manager.InitBlueMoonSet();
        }

        if (GUI.Button(new Rect(Screen.width - 300, 350, 100, 50), "블루문시작"))
        {
            BattleManager.initState = BattleInitState.Bluemoon;
            manager.Release();
            manager.Init(true);
        }

        if(GUI.Button(new Rect(Screen.width/2 , Screen.height/2, 50, 50), "재시작"))
        {
            GameManager.Manager.LoadScene(GameScene.Battle);
        }
    }
}
