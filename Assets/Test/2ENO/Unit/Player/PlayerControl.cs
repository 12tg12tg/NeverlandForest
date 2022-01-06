using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerMoveAnimation
{
    Idle,
    Walk,
}

public class PlayerControl : MonoBehaviour, IAttackable
{ 

    void Update()
    {
        // 테스트용, 던전 이동씬에서만 동작!
        //isMove = false;
        //if (Input.GetKey(KeyCode.D))
        //{
        //    var pos = Vector3.forward * 5f * Time.deltaTime;
        //    transform.position += pos;
        //    transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
        //    isMove = true;
        //}
        //else if(Input.GetKey(KeyCode.A))
        //{
        //    var pos = -Vector3.forward * 5f * Time.deltaTime;
        //    transform.position += pos;
        //    transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        //    isMove = true;
        //}
    }

    public void OnAttacked(BattleCommand attacker)
    {
        // 피격 애니메이션 몇초간 실행
    }
}

    //// 작동 x
    //public void OnPlayerMove(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context.ReadValue<Vector2>());
    //}