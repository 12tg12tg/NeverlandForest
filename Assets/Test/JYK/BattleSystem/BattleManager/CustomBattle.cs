using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBattle : MonoBehaviour
{
    public bool useCustomMode;
    public int waveNum;

    public List<bool> haveMonster1;
    public List<bool> haveMonster2;
    public List<bool> haveMonster3;

    public List<MonsterPoolTag> cwave1;
    public List<MonsterPoolTag> cwave2;
    public List<MonsterPoolTag> cwave3;
}
