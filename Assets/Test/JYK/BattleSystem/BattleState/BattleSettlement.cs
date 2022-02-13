using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleSettlement : State<BattleState>
{
    private BattleManager manager;
    private float timer;
    readonly float time = 3f;
    public BattleSettlement(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();

        // �������� �ǵ� ȸ�� ���
        manager.monsters.ForEach(n =>
        {
            if (!n.IsBurn)
            {
                n.Shield++;
                if (n.Shield > n.maxSheild)
                    n.Shield = n.maxSheild;
                n.uiLinker.UpdateSheild(n.Shield);
            }
            else
            {
                n.IsBurn = false;
            }
        });

        // �ǵ��. ����� �Ǳ�.
        manager.AllMonsterDebuffCheck();

        // �ѹ��� �������� Ȯ��.
        var monsterlist = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (monsterlist.Count == 0) // ������ ���� 0����
        {
            if (manager.waveLink.IsAllWaveClear()) // ���̺� ��� Ŭ����
            {
                manager.uiLink.turnSkipTrans.SetActive(false);
                manager.uiLink.progressTrans.SetActive(false);
                manager.uiLink.PrintMessage($"�¸�!", 2.5f, () =>
                {
                    manager.Win();
                });
            }
            else
            {
                //����.
                manager.uiLink.PrintMessage($"{manager.Turn}�� ��", 0.8f, () =>
                {
                    var turn = ++manager.Turn;

                    // ���̺� ������Ʈ ( �˾Ƽ� ���� Ȯ�� �Ŀ��̺� ������Ʈ ��. )
                    manager.waveLink.UpdateWave();

                    manager.uiLink.PrintMessage($"{manager.Turn}�� ����", 0.8f, () =>
                    {
                        if (manager.isPlayerFirst)
                            FSM.ChangeState(BattleState.Player);
                        else
                            FSM.ChangeState(BattleState.Monster);
                    });
                });
            }
        }
        else
        {
            //����.
            manager.uiLink.PrintMessage($"{manager.Turn}�� ��", 0.8f, () =>
            {
                var turn = ++manager.Turn;

            // ���̺� ������Ʈ ( �˾Ƽ� ���� Ȯ�� �Ŀ��̺� ������Ʈ ��. )
            manager.waveLink.UpdateWave();

                manager.uiLink.PrintMessage($"{manager.Turn}�� ����", 0.8f, () =>
                {
                    if (manager.isPlayerFirst)
                        FSM.ChangeState(BattleState.Player);
                    else
                        FSM.ChangeState(BattleState.Monster);
                });
            });
        }
    }

    public override void Release()
    {

    }

    public override void Update()
    {
        //timer += Time.deltaTime;
        //if(timer > time)
        //{
        //    FSM.ChangeState(BattleState.Player);
        //}
    }
    public override void LateUpdate()
    {
    }
    public override void FixedUpdate()
    {
    }
}
