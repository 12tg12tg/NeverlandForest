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
        if (GUILayout.Button("��繮X, ����������X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            manager.Init(false);
        }
        if (GUILayout.Button("��繮X, ����������O", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            manager.Init(false, true);
        }
        if (GUILayout.Button("��繮O, ����������X", GUILayout.Width(200f), GUILayout.Height(100f)))
        {
            manager.Init(true);
        }

        if (GUILayout.Button("Lantern -12", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.ConsumeLantern(12);
        }
        if (GUILayout.Button("Lantern +12", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ConsumeManager.FullingLantern(12);
        }

        if (GUI.Button(new Rect(Screen.width - 105, 0, 100, 100), "�ð���"))
        {
            BottomUIManager.Instance.curObstacleType = ObstacleType.Lasso;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 100, 100, 100), "�κ�Ʈ��"))
        {
            if (!Inventory_Virtual.instance.isLasso)
                BottomUIManager.Instance.curObstacleType = ObstacleType.BoobyTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 200, 100, 100), "����Ʈ��"))
        {
            if (!Inventory_Virtual.instance.isLasso)
                BottomUIManager.Instance.curObstacleType = ObstacleType.WoodenTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 300, 100, 100), "����Ʈ��"))
        {
            if (!Inventory_Virtual.instance.isLasso)
                BottomUIManager.Instance.curObstacleType = ObstacleType.ThornTrap;
        }
        if (GUI.Button(new Rect(Screen.width - 105, 400, 100, 100), "��ֹ�"))
        {
            if (!Inventory_Virtual.instance.isLasso)
                BottomUIManager.Instance.curObstacleType = ObstacleType.Barrier;
        }
        if (GUI.Button(new Rect(Screen.width - 300, 0, 100, 50), "���� ����"))
        {
            var list = new List<MonsterUnit>(manager.monsters);
            list.AddRange(manager.waveLink.wave1);
            list.AddRange(manager.waveLink.wave2);
            list.AddRange(manager.waveLink.wave3);
            manager.waveLink.wave1.Clear();
            manager.waveLink.wave2.Clear();
            manager.waveLink.wave3.Clear();
            list.ToList().ForEach(n => { if (n != null) n.Release(); });
            manager.FSM.ChangeState(BattleState.Monster);
        }
        if (GUI.Button(new Rect(Screen.width - 300, 50, 100, 50), "���� ���� �� ��ȯ"))
        {
            manager.monsters.ForEach(n => n.Release());
            SceneManager.LoadScene("As_RandomMap");
        }
    }
}
