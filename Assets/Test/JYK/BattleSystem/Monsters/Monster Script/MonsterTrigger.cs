using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    private PlayerAction boyAction;
    [SerializeField] private MonsterUnit monsterUnit;

    [SerializeField] private Collider arrowTrigger;
    [SerializeField] private Collider moveTrigger;

    private void Start()
    {
        boyAction = BattleManager.Instance.boy.FSM.GetState(CharacterBattleState.Action) as PlayerAction;
        if(boyAction == null)
            Debug.LogError("BoyAction is null");
    }

    // Trigger Enable
    public void EnableHitTrigger()
    {
        arrowTrigger.enabled = true;
    }

    public void DisableHitTrigger()
    {
        arrowTrigger.enabled = false;
    }

    public void EnableMoveTrigger()
    {
        moveTrigger.enabled = true;
    }

    public void DisableMoveTrigger()
    {
        moveTrigger.enabled = false;
    }


    // Event Func
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Arrow"))
        {
            if (BattleManager.Instance.boy.command.skill.SkillTableElem.name == "���߰���")
            {
                if (other.GetComponent<Arrow>().isFinalShot)
                {
                    boyAction.isAttackMotionEnd = true;
                    DisableHitTrigger();
                }
                monsterUnit.PlayHitAnimation();
                monsterUnit.OnAttacked(BattleManager.Instance.boy.command);
            }
            else
            {
                boyAction.isAttackMotionEnd = true;
                DisableHitTrigger();
            }
        }
        else if (other.CompareTag("Trap"))
        {
            var obs = other.GetComponent<Obstacle>();
            obs.tile.obstacle = null;
            ObstacleDebuff debuff;
            switch (obs.type)
            {
                case TrapTag.Snare:
                    debuff = new ObstacleDebuff(obs, monsterUnit);
                    monsterUnit.obsDebuffs.Add(debuff);
                    break;

                case TrapTag.WoodenTrap:
                case TrapTag.ThornTrap:
                    debuff = new ObstacleDebuff(obs, monsterUnit);
                    monsterUnit.obsDebuffs.Add(debuff);

                    monsterUnit.PlayHitAnimation();
                    break;


                case TrapTag.BoobyTrap:
                    break;
                case TrapTag.Fence:
                    break;
            }


        }
    }
}
