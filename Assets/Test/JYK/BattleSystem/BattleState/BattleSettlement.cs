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
        //����.
        manager.PrintMessage($"{manager.Turn}�� ��", 0.8f, () =>
        {
            var turn = ++manager.Turn;

            // ���̺� ������Ʈ ( �˾Ƽ� ���� Ȯ�� �Ŀ��̺� ������Ʈ ��. )
            manager.UpdateWave();

            manager.PrintMessage($"{manager.Turn}�� ����", 0.8f, () =>
            {
                if(manager.isPlayerFirst)
                    FSM.ChangeState(BattleState.Player);
                else
                    FSM.ChangeState(BattleState.Monster);
            });
        });

        //�ǵ��. ����� �Ǳ�.
        manager.AllMonsterDebuffCheck();

        //�ٽ� �÷��̾�������
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
