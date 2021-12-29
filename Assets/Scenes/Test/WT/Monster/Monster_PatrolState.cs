using UnityEngine;

public class Monster_PatrolState : State<MonsterState>
{
    private Transform playerTransform;
    private int rand;
    private float timer;
    private float checktime = 3f;
    public MonsterDirection curdirection;
    public override void Init()
    {
        Debug.Log("MonsterPatrol");
        playerTransform = GameObject.FindWithTag("Player").transform;
        curdirection = MonsterDirection.Forward;
    }
    public override void Release()
    {
    }

    public override void Update()
    {
        var dis = Vector2.Distance(transform.position, playerTransform.position);
        timer += Time.deltaTime;
        if (timer > checktime)
        {
            if (dis > 5f)
            {
                Move(curdirection);
                timer = 0f;
            }
            else
            {
                FSM.ChangeState(MonsterState.Attack);
            }
        }
    }
    public override void FixedUpdate()
    {
    }
    public override void LateUpdate()
    {
    }

    public void Move(MonsterDirection direction)
    {
        var stats = gameObject.GetComponent<MonsterStats>();
        switch (direction)
        {
            case MonsterDirection.Forward:
                GoFoward(stats);
                break;
            case MonsterDirection.Backward:
                GoBack(stats);
                break;
            case MonsterDirection.Left:
                GoLeft(stats);
                break;
            case MonsterDirection.Right:
                GoRight(stats);
                break;
            case MonsterDirection.Forward_Left:
                GoForward_Left(stats);
                break;
            case MonsterDirection.Forward_Right:
                GoForward_Right(stats);
                break;
            default:
                break;
        }
    }

    private void GoBack(MonsterStats stats)
    {
        Debug.Log("뒤로");
        var checkPos = new Vector2(stats.tilePos.x, stats.tilePos.y + 1);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }

    private void GoLeft(MonsterStats stats)
    {
        Debug.Log("왼쪽으로");
        var checkPos = new Vector2(stats.tilePos.x - 1, stats.tilePos.y);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }

    private void GoRight(MonsterStats stats)
    {
        Debug.Log("오른쪽으로");
        var checkPos = new Vector2(stats.tilePos.x + 1, stats.tilePos.y);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }

    private void GoForward_Left(MonsterStats stats)
    {
        Debug.Log("앞으로왼쪽으로");
        var checkPos = new Vector2(stats.tilePos.x - 1, stats.tilePos.y - 1);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }

    private void GoForward_Right(MonsterStats stats)
    {
        Debug.Log("앞으로오른쪽으로");
        var checkPos = new Vector2(stats.tilePos.x + 1, stats.tilePos.y - 1);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }

    private void GoFoward(MonsterStats stats)
    {
        Debug.Log("앞으로");
        var checkPos = new Vector2(stats.tilePos.x, stats.tilePos.y - 1);
        var NewPos = stats.tilemaker.GetTile(checkPos);
        if (NewPos != null && !NewPos.isObstacle)
        {
            stats.tilePos = checkPos;
            transform.position = NewPos.transform.position;
        }
    }
}
