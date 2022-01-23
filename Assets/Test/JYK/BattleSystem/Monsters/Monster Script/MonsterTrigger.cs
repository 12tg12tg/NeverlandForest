using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    private PlayerAction boyAction;
    private MonsterUnit monsterUnit;

    private void Start()
    {
        monsterUnit = gameObject.GetComponent<MonsterUnit>();
        boyAction = BattleManager.Instance.boy.FSM.GetState(CharacterBattleState.Action) as PlayerAction;
        if(boyAction == null || monsterUnit == null)
            Debug.LogError("Somthing is null");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Arrow"))
        {
            if (BattleManager.Instance.boy.command.skill.SkillTableElem.name == "집중공격")
            {
                if(other.GetComponent<Arrow>().isFinalShot)
                {
                    boyAction.isAttackMotionEnd = true;
                    monsterUnit.trigger.enabled = false;
                }
                monsterUnit.PlayHitAnimation();
                monsterUnit.OnAttacked(BattleManager.Instance.boy.command);
            }
            else
            {
                boyAction.isAttackMotionEnd = true;
                monsterUnit.trigger.enabled = false;
            }
        }        
        else if (other.CompareTag("Trap"))
            Destroy(other.gameObject);
    }
}
