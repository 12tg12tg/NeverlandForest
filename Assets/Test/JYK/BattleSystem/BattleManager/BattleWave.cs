using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWave : MonoBehaviour
{
    public TileMaker tileMaker;
    public BattleManager manager;

    public List<MonsterUnit> wave1 = new List<MonsterUnit>();
    public List<MonsterUnit> wave2 = new List<MonsterUnit>();
    public List<MonsterUnit> wave3 = new List<MonsterUnit>();

    public int totalWave;
    public int curWave;

    public IEnumerable<MonsterUnit> AliveMonsters
        => manager.monsters.Union(wave1).Union(wave2).Union(wave3).Where(n => n != null);


    private void Start()
    {
        manager = BattleManager.Instance;
        tileMaker = TileMaker.Instance;
    }

    public bool IsAllWaveClear()
    {
        return wave1.Count == 0 && wave2.Count == 0 && wave3.Count == 0;
    }
    public bool IsReadyToNextWave(List<MonsterUnit> nextWave)
    {
        for (int i = 0; i < nextWave.Count; i++)
        {
            if (nextWave[i] == null)
                continue;

            var tile = tileMaker.GetTile(new Vector2(i, 6));
            if (!tile.CanStand)
                return false;
        }
        return true;
    } //마지막 열에 몬스터 한마리씩 설 자리가 있다면 true 반환.

    public void StartWave(int wave)
    {
        List<MonsterUnit> temp = null;
        if (wave == 1)
            temp = wave1;
        else if (wave == 2)
            temp = wave2;
        else
            temp = wave3;

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] == null)
                continue;
            manager.monsters.Add(temp[i]);
            var tempForCoroutine = temp[i];
            var tilePos = new Vector2(i, 6);
            tempForCoroutine.ObstacleAdd(tilePos);
            tempForCoroutine.SetMoveUi(true);
            manager.tileLink.MoveUnitOnTile(tilePos, tempForCoroutine, tempForCoroutine.PlayMoveAnimation,
                () => { tempForCoroutine.PlayIdleAnimation(); tempForCoroutine.SetMoveUi(false); });
        }
        temp.Clear();

    } //매개변수 웨이브를 전투에 입장시키기.

    public void SetWavePosition(List<MonsterUnit> waveList, bool useCoroutine = false)
    {
        if (waveList.Count == 0)
            return;

        int wave;
        if (waveList == wave1)
            wave = 1;
        else if (waveList == wave2)
            wave = 2;
        else
            wave = 3;

        int count = waveList.Count;
        var remainWave = wave - curWave;

        var basePos = tileMaker.GetTile(new Vector2(0, 6)).transform.position;
        var leftPos = tileMaker.GetTile(new Vector2(0, 5)).transform.position;
        var upPos = tileMaker.GetTile(new Vector2(1, 6)).transform.position;
        var spacingX = basePos.x - leftPos.x;
        var spacingZ = upPos.z - basePos.z;

        //if (count == 2)
        //    basePos += new Vector3(0f, 0f, spacingZ / 2);

        for (int i = 0; i < count; i++)
        {
            if (waveList[i] == null)
                continue;

            var curPos = waveList[i].transform.position;
            var newPos = basePos + new Vector3(spacingX * remainWave, 0f, i * spacingZ);
            if (!useCoroutine)
            {
                waveList[i].transform.position = newPos;
                waveList[i].uiLinker.linkedUi.MoveUi();
            }
            else
            {
                var tempForCoroutine = waveList[i];
                waveList[i].PlayMoveAnimation();
                waveList[i].SetMoveUi(true);
                StartCoroutine(Utility.CoTranslate(waveList[i].transform, curPos, newPos, 1f,
                    () => { tempForCoroutine.PlayIdleAnimation(); tempForCoroutine.SetMoveUi(false); }));
            }
        }
    }

    public void UpdateWave() //조건 확인
    {
        if (curWave == 3)
            return;

        if(totalWave == 2)
            wave3.Clear();


        List<MonsterUnit> nextWave = null;
        if (curWave == 0)
            nextWave = wave1;
        else if (curWave == 1)
            nextWave = wave2;
        else if (curWave == 2)
            nextWave = wave3;

        if(nextWave.All(n=>n==null))
        {
            nextWave.Clear();
            curWave++;
            if (curWave == 0)
                nextWave = wave1;
            else if (curWave == 1)
                nextWave = wave2;
            else if (curWave == 2)
                nextWave = wave3;
        }

        if (!IsReadyToNextWave(nextWave))
            return;
        if (manager.turn != 1 && manager.turn - manager.preWaveTurn < 1)
            return;
        else
            manager.preWaveTurn = manager.turn;

        curWave++;
        if (curWave == 1)
        {
            StartWave(1);
            SetWavePosition(wave2, true);
            SetWavePosition(wave3, true);
        }
        else if (curWave == 2)
        {
            StartWave(2);
            SetWavePosition(wave3, true);
        }
        else if (curWave == 3)
        {
            StartWave(3);
        }
    }

    public void SetAllMonsterFollowUI(bool moveUI)
    {
        var list = AliveMonsters;
        foreach (var unit in list)
        {
            unit.SetMoveUi(moveUI);
        }
    }
}
