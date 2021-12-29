using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHuntingUnit : UnitBase
{
    private PlayerStats playerStat;
    private Coroutine tileMoveCoroutine;
    private Animator playerAnimation;
    private PlayerMoveAnimation curAnimation;

    private void Start()
    {
        playerAnimation = gameObject.GetComponent<Animator>();
        playerStat = gameObject.GetComponent<PlayerStats>();
    }

    void Update()
    {

    }
    public void TileMove(Vector2 index, Vector3 position)
    {
        playerStat.Pos = tileMoveCoroutine == null ? index : playerStat.Pos;
        tileMoveCoroutine ??= StartCoroutine(Utility.CoTranslate2(transform, transform.position, position, 1f,
            TileMoveAnimation, () => tileMoveCoroutine = null));
    }

    private void TileMoveAnimation(Vector3 pos, bool isMove, bool tileMoveEnd)
    {
        AnimationChange(isMove);
        if (tileMoveEnd)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(pos);
        }
    }

    private void AnimationChange(bool isMove)
    {
        if (isMove && (curAnimation != PlayerMoveAnimation.Walk))
        {
            curAnimation = PlayerMoveAnimation.Walk;
            playerAnimation.SetTrigger("Walk");
        }
        else if (!isMove && (curAnimation != PlayerMoveAnimation.Idle))
        {
            curAnimation = PlayerMoveAnimation.Idle;
            playerAnimation.SetTrigger("Idle");
        }
    }
}
