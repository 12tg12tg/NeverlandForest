using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DateEvent
{
    None,
    BlueMoon,
    WitchEffect,
}

public enum TimeState
{
    None,
    NightTime,
    DayTime,
}
public enum LivingState
{
    None,
    Playing,
    GameOver
}
public enum LanternState
{   
    None,
    Level1,//여기가 부족단계
    Level2,
    Level3,
    Level4, //여기가 풀
}


public static class ConsumeManager
{
    private static TimeState curTimeState;
    private static LanternState curLanternState;
    public static TimeState CurTimeState
    {
        set => curTimeState = value;
        get => curTimeState;
    }
    public static LanternState CurLanternState
    {
        set => curLanternState = value;
        get => curLanternState;
    }
    

    public static void init()
    {
        TimeStateChange();
        LanternStateChange();
        TirednessChange(); //최초의 피로도는 ChangeableMaxStamina;
    }
    public static void SaveConsumableData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void LoadConsumableData()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void GetthingHunger(int hungercount) //굶주림 증가 -> 자연소비로 해야됨.
    {
        if (Vars.UserData.uData.ChangeableMaxStamina >0)
        {
            Vars.UserData.uData.Hunger += hungercount;
            ChangeableMaxStaminChange();
        }
    }
    public static void RecoveryHunger(int hungercount) //굶주림 감소 -> 스태미너의 최대치를 결정함
    {
        Vars.UserData.uData.Hunger -= hungercount;
        if (Vars.UserData.uData.Hunger<0)
        {
            Vars.UserData.uData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
    }
    public static void  RecoveryTiredness() //피로도 회복 = 스태미너를 최대치까지 회복함(수면)
    {
        // 회복할수 있는 최대치 까지 회복 시키는 거기때문에 매개변수는 받을필요가 없을 것 같고
        // 회복된 수치에 비례해서 시간을 소비하는 개념으로 가야될것 같다.

        var time = CampManager.Instance.RecoverySleepTime;
        var rimittime = Vars.UserData.uData.BonfireHour *60;
        Debug.Log($"time{time}");
        Debug.Log($"rimittime{rimittime}");

        if (rimittime>=time)
        {
            if (Vars.UserData.uData.CurStamina < Vars.UserData.uData.ChangeableMaxStamina)
            {
                var recoverValue = Vars.UserData.uData.ChangeableMaxStamina - Vars.UserData.uData.Tiredness;
                var afterValue = Vars.UserData.uData.ChangeableMaxStamina / 10; // 30분당 이만큼 
                var finalValue = time / 30 * afterValue;
                if (Vars.UserData.uData.ChangeableMaxStamina > Vars.UserData.uData.Tiredness)
                {
                    //TimeUp((int)recoverValue * 6); //기존 충전량
                    TimeUp(time); //변경된 충전량
                    Debug.Log($"finalvalue{time}");
                }
                //Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina; // 회복기존꺼
                Vars.UserData.uData.Tiredness += finalValue; // 회복변경된거
                if (Vars.UserData.uData.Tiredness > Vars.UserData.uData.ChangeableMaxStamina)
                {
                    Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina;
                }
                //스태미나 10회복당 1시간 개념이였으니깐 1회복당 6분의 개념
                CurStaminaChange();
            }
        }
        else
        {
            Debug.Log("모닥불의 시간이 부족합니다");
        }
    }
    public static void GettingTired(float gettingTired) //피로도 증가
    {
        Vars.UserData.uData.Tiredness -= gettingTired;
        if (Vars.UserData.uData.Tiredness < 0)
        {
            Vars.UserData.uData.Tiredness = 0;
            //eventbus에 게임오버를 보내주자.
            //eventbus에 gamestate는 어디서 만들지? gameManger에 만들어져 있던가?
            EventBus<LivingState>.Publish(LivingState.GameOver);
        }
        CurStaminaChange();
    }
    private static void ChangeableMaxStaminChange()
    {
        Vars.UserData.uData.ChangeableMaxStamina = Vars.maxStamina - Vars.UserData.uData.Hunger;
    }
    private static void TirednessChange()
    {
        Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina;
    }
    private static void CurStaminaChange()
    {
        Vars.UserData.uData.CurStamina = Vars.UserData.uData.Tiredness;
    }
    public static void FullingLantern(int oil)
    {
        if (Vars.UserData.uData.LanternCount<18)
        {
            Vars.UserData.uData.LanternCount += oil;
            LanternStateChange();
        }
       
    }
    private static void RecoverHp(PlayerType type,float recovery)
    {
        if (type == PlayerType.Boy)
        {
            Vars.UserData.uData.HunterHp += recovery;
            if (Vars.UserData.uData.HunterHp >Vars.hunterMaxHp)
            {
                Vars.UserData.uData.HunterHp = Vars.hunterMaxHp;
            }
        }
        else if (type == PlayerType.Girl)
        {
            Vars.UserData.uData.HerbalistHp += recovery;
            if (Vars.UserData.uData.HerbalistHp > Vars.herbalistMaxHp)
            {
                Vars.UserData.uData.HerbalistHp = Vars.herbalistMaxHp;
            }
        }
    }
    public static void ConsumeLantern(int oil)
    {
        if (Vars.UserData.uData.LanternCount>0)
        {
            Vars.UserData.uData.LanternCount -= oil;
            LanternStateChange();
        }
    }
    private static void LanternStateChange()
    {
        var count = Vars.UserData.uData.LanternCount;
        TimeStateChange();
        switch (curTimeState)
        {
            case TimeState.None:
                break;
            case TimeState.NightTime:
                SetNightLaternState(count);
                break;
            case TimeState.DayTime:
                SetDayLaternState(count);
                break;
            default:
                break;
        }
        Vars.UserData.uData.lanternState = curLanternState;
    }

    private static void SetDayLaternState(float count)
    {
        if (count <= 17 && count > 14) //15,16,17
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 14 && count > 10)//11,12,13,14
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 10 && count > 5)//6,7,8,9,10
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 5 && count > 0)//1,2,3,4,5
        {
            curLanternState = LanternState.Level2;
        }
        else if (count == 0)
        {
            curLanternState = LanternState.Level1;
        }
    }
    private static void SetNightLaternState(float count)
    {
        if (count <= 17 && count > 14) //15,16,17
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 14 && count > 10)//11,12,13,14
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 10 && count > 5)//6,7,8,9,10
        {
            curLanternState = LanternState.Level2;
        }
        else if (count <= 5 && count > 0)//1,2,3,4,5
        {
            curLanternState = LanternState.Level1;
        }
        else if (count == 0)
        {
            curLanternState = LanternState.None;
        }
    }
    private static void TimeStateChange()
    {   
        if(Vars.UserData.uData.CurIngameHour <= 12)
        {
            if(curTimeState != TimeState.DayTime)
            {
                EventBus<TimeState>.Publish(TimeState.DayTime);
            }
            curTimeState = TimeState.DayTime;
        }
        else if(Vars.UserData.uData.CurIngameHour <= 24)
        {
            if (curTimeState != TimeState.NightTime)
            {
                EventBus<TimeState>.Publish(TimeState.NightTime);
            }
            curTimeState = TimeState.NightTime;
        }
        Debug.Log($"Vars.UserData.uData.CurIngameHour{Vars.UserData.uData.CurIngameHour}");
    }

    public static void TimeUp(float minute, float hour=0)
    //시간보다는 분을 더 자주쓸거같아서 hour은 디폴트 매개변수로 두었습니당!
    {
        Vars.UserData.uData.CurIngameHour += hour;
        Vars.UserData.uData.CurIngameMinute += minute;

        float consumeTotalMinute = 60 * hour + minute;
        float consumeStamina = consumeTotalMinute / 30;
        //30분당 스태미나 1(n)소비 인데 어떻게해야 깔끔하게 정리 할 수 있을까
        Debug.Log($"consumeTotalMinute {consumeTotalMinute}");
        Debug.Log($"consumeStamina {consumeStamina}");
        GettingTired(consumeStamina);
      
        while(Vars.UserData.uData.CurIngameMinute >= Vars.maxIngameMinute)
        {
            Vars.UserData.uData.CurIngameMinute -= Vars.maxIngameMinute;
            Vars.UserData.uData.CurIngameHour++;
        }
        while(Vars.UserData.uData.CurIngameHour >= Vars.maxIngameHour)
        {
            Vars.UserData.uData.CurIngameHour -= Vars.maxIngameHour;
            DateUp();
        }
        TimeStateChange();
    }
    public static void RecoveryTimeUp(int minute, int hour=0)
    {
        Vars.UserData.uData.CurIngameHour += hour;
        Vars.UserData.uData.CurIngameMinute += minute;
        while (Vars.UserData.uData.CurIngameMinute >= Vars.maxIngameMinute)
        {
            Vars.UserData.uData.CurIngameMinute -= Vars.maxIngameMinute;
            Vars.UserData.uData.CurIngameHour++;
        }
        while (Vars.UserData.uData.CurIngameHour >= Vars.maxIngameHour)
        {
            Vars.UserData.uData.CurIngameHour -= Vars.maxIngameHour;
            DateUp();
        }
        TimeStateChange();
    }
    public static void DateUp()
    {
        Vars.UserData.uData.Date++;
        if(Vars.UserData.uData.Date % 15 == 0)
        {
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon);
        }

        if(Vars.UserData.uData.Date > 2)
        {
            EventBus<DateEvent>.Publish(DateEvent.WitchEffect);
        }
    }
    public static void ConsumeBonfireTime(float minute, float hour =0)
    {
        var totalTime = Vars.UserData.uData.BonfireHour * 60; // 분단위로 계산

        totalTime -= minute;
        totalTime -= 60 * hour;
        if (totalTime<0)
        {
            totalTime = 0;
        }
        Vars.UserData.uData.BonfireHour = totalTime/60;
        CampManager.Instance.ChangeBonTime();
    }

    public static void RecoveryBonFire(float minute, float hour = 0)
    {
        var totalTime = Vars.UserData.uData.BonfireHour * 60; // 분단위로 계산
        totalTime += minute;
        totalTime += 60 * hour;
        Vars.UserData.uData.BonfireHour = totalTime / 60;
        CampManager.Instance.ChangeBonTime();

    }

    private static void BlueMoon(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : 블루문 실행");
    }
    private static void WitchEffect(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : 마녀 이펙트 실행");
    }
    private static void DayTime(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : DayTime 실행");
    }
    private static void NightTime(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : NightTime 실행");
    }

    public static void CostDataReset()
    {
        Vars.UserData.uData.Hunger = 0;
        Vars.UserData.uData.CurIngameHour = 0;
        Vars.UserData.uData.CurIngameMinute = 0;
        Vars.UserData.uData.LanternCount = 18;
        Vars.UserData.uData.Date = 0;
        Vars.UserData.uData.Tiredness = 100;
        Vars.UserData.uData.HunterHp = 100f;
        Vars.UserData.uData.HerbalistHp = 100f;

    }

    private static void Referece()
    {
        EventBus<DateEvent>.Subscribe(DateEvent.BlueMoon, BlueMoon);
        EventBus<DateEvent>.Subscribe(DateEvent.WitchEffect, WitchEffect);

        EventBus<TimeState>.Subscribe(TimeState.DayTime, DayTime);
        EventBus<TimeState>.Subscribe(TimeState.NightTime, NightTime);


        EventBus<DateEvent>.Unsubscribe(DateEvent.BlueMoon, BlueMoon);
        EventBus<DateEvent>.Unsubscribe(DateEvent.WitchEffect, WitchEffect);

        EventBus<TimeState>.Unsubscribe(TimeState.DayTime, DayTime);
        EventBus<TimeState>.Unsubscribe(TimeState.NightTime, NightTime);
        EventBus<DateEvent>.ResetEventBus();

    }
}
