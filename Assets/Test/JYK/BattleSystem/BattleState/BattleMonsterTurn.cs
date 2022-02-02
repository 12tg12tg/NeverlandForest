using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleMonsterTurn : State<BattleState>
{
    private BattleManager manager;
    private float timer = 0;
    private bool canEndState;
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        // 1. �ൿ�� ���Ͱ� �ִ��� Ȯ��
        var list = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (list.Count == 0)
        {
            if(manager.waveLink.IsAllWaveClear())
                manager.uiLink.PrintMessage($"�¸�!", 2.5f, () => SceneManager.LoadScene("AS_RandomMap"));
            else
            {
                manager.uiLink.PrintMessage("�ൿ�� ���� ����!", 1f, () => canEndState = true);
            }
        }
        else
        {
            manager.uiLink.PrintMessage("���� ��", 1f, () => canEndState = true);

            manager.MonsterActionQueue.Clear();

            foreach (var monster in list)
            {
                var command = monster.command;
                if (command != null)
                    manager.MonsterActionQueue.Enqueue(command);
            }

            /*ť ���ı��� �� ���� ���� ���ϱ� - ���� Ȯ�� �� ������Ʈ*/
        }
    }

    public override void Release()
    {
        Debug.Log("Battle Monster Release");
    }

    public override void Update()
    {
        if (canEndState)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0f;
                FSM.ChangeState(BattleState.Action);
            }
        }
    }
    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {

    }
}
