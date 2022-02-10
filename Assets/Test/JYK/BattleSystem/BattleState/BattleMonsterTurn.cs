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
    private bool noMonster;
    public BattleMonsterTurn(BattleManager manager)
    {
        this.manager = manager;
    }

    public override void Init()
    {
        BottomUIManager.Instance.ItemListInit();
        // 1. �ൿ�� ���Ͱ� �ִ��� Ȯ��
        noMonster = false;
        var stageMonster = manager.monsters.Where(n => n.State != MonsterState.Dead).ToList();
        if (stageMonster.Count == 0)
        {
            if (manager.waveLink.IsAllWaveClear())
            {
                manager.uiLink.PrintMessage($"�¸�!", 2.5f, () =>
                {
                    manager.uiLink.turnSkipTrans.SetActive(false);
                    manager.uiLink.progressTrans.SetActive(false);

                    // �ڽ¸�
                    if (manager.isTutorial) // Ʃ�丮��
                    {
                        manager.tutorial.isWin = true;
                        manager.boy.PlayWinAnimation();
                        manager.girl.PlayWinAnimation();
                        manager.directingLink.LandDownLantern();
                        manager.uiLink.OpenRewardPopup();
                    }
                    else // ����
                    {
                        manager.uiLink.OpenRewardPopup();
                        manager.boy.PlayWinAnimation();
                        manager.girl.PlayWinAnimation();
                        manager.directingLink.LandDownLantern();
                        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.Battle);
                    }

                });
            }
            else
            {
                noMonster = true;
                manager.uiLink.PrintMessage("�ൿ�� ���� ����!", 1f,
                    () =>
                    {
                        canEndState = true;
                    });
            }
        }
        else
        {
            manager.uiLink.PrintMessage("���� ��", 1f, () => canEndState = true);

            manager.MonsterActionQueue.Clear();

            foreach (var monster in stageMonster)
            {
                var command = monster.command;
                if (command != null)
                    manager.MonsterActionQueue.Enqueue(command);
            }

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
            if (timer > 1.2f) // 1�ʿ��� 1.2�ʷ� �ø�. â �ݱ������ Settlement���� â�� �ٽ� ȣ���ع����� �ᱹ �ٷ� �ݱ�� �ڷ�ƾ �ߴ܉����.
            {
                timer = 0f;
                if (!noMonster)
                {
                    FSM.ChangeState(BattleState.Action);
                }
                else
                {
                    if (manager.isPlayerFirst)
                        FSM.ChangeState(BattleState.Settlement);
                    else
                        FSM.ChangeState(BattleState.Player);
                }
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
