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
public enum GameState
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
    private static TimeState curTimeState = TimeState.None;
    private static LanternState curLanternState = LanternState.Level4;
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
        if (Vars.UserData.ChangeableMaxStamina >0)
        {
            Vars.UserData.Hunger += hungercount;
            ChangeableMaxStaminChange();
        }
    }
    public static void RecoveryHunger(int hungercount) //굶주림 감소 -> 스태미너의 최대치를 결정함
    {
        Vars.UserData.Hunger -= hungercount;
        if (Vars.UserData.Hunger<0)
        {
            Vars.UserData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
    }
    public static void  RecoveryTiredness() //피로도 회복 = 스태미너를 최대치까지 회복함(수면)
    {
        // 회복할수 있는 최대치 까지 회복 시키는 거기때문에 매개변수는 받을필요가 없을 것 같고
        // 회복된 수치에 비례해서 시간을 소비하는 개념으로 가야될것 같다.
        if (Vars.UserData.CurStamina<Vars.UserData.ChangeableMaxStamina )
        {
            var recoverValue = Vars.UserData.ChangeableMaxStamina - Vars.UserData.Tiredness;
            if (Vars.UserData.ChangeableMaxStamina > Vars.UserData.Tiredness)
            {
                TimeUp(recoverValue * 6);
            }
            Vars.UserData.Tiredness = Vars.UserData.ChangeableMaxStamina; // 회복
            if (Vars.UserData.Tiredness > Vars.UserData.ChangeableMaxStamina)
            {
                Vars.UserData.Tiredness = Vars.UserData.ChangeableMaxStamina;
            }
            //스태미나 10회복당 1시간 개념이였으니깐 1회복당 6분의 개념
            CurStaminaChange();
        }
    }
    public static void GettingTired(int gettingTired) //피로도 증가
    {
        Vars.UserData.Tiredness -= gettingTired;
        if (Vars.UserData.Tiredness < 0)
        {
            Vars.UserData.Tiredness = 0;
            //eventbus에 게임오버를 보내주자.
            //eventbus에 gamestate는 어디서 만들지? gameManger에 만들어져 있던가?
            EventBus<GameState>.Publish(GameState.GameOver);
        }
        CurStaminaChange();
    }
    private static void ChangeableMaxStaminChange()
    {
        Vars.UserData.ChangeableMaxStamina = Vars.maxStamina - Vars.UserData.Hunger;
    }
    private static void TirednessChange()
    {
        Vars.UserData.Tiredness = Vars.UserData.ChangeableMaxStamina;
    }
    private static void CurStaminaChange()
    {
        Vars.UserData.CurStamina = Vars.UserData.Tiredness;
    }
    private static void FullingLantern(int oil)
    {
        Vars.UserData.LanternCount += oil;
        LanternStateChange();
    }
    private static void RecoverHp(PlayerType type,float recovery)
    {
        if (type == PlayerType.Boy)
        {
            Vars.UserData.hunterHp += recovery;
            if (Vars.UserData.hunterHp>Vars.hunterMaxHp)
            {
                Vars.UserData.hunterHp = Vars.hunterMaxHp;
            }
        }
        else if (type == PlayerType.Girl)
        {
            Vars.UserData.herbalistHp += recovery;
            if (Vars.UserData.herbalistHp > Vars.herbalistMaxHp)
            {
                Vars.UserData.herbalistHp = Vars.herbalistMaxHp;
            }
        }
    }
    private static void ConsumeLantern(int oil)
    {   
        // 사용하는 아이템에 따라서 oil의 수치값을 정해주면될것같다.
        Vars.UserData.LanternCount -= oil;
        LanternStateChange();
    }
    private static void LanternStateChange()
    {
        var count = Vars.UserData.LanternCount;
        if (count <= 17 &&count>14) //15,16,17
        {
            curLanternState = LanternState.Level4;
        }
        else if (count<=14 && count>10)//11,12,13,14
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
        else //0
        {
            curLanternState = LanternState.None;
        }
    }
    private static void TimeStateChange()
    {   
        //현재 인호가 구현해둔 방식은 원 하나에 24시간을 들고있나봄.
        if(Vars.UserData.CurIngameHour <= 12)
        {
            if(curTimeState != TimeState.DayTime)
            {
                EventBus<TimeState>.Publish(TimeState.DayTime);
            }
            curTimeState = TimeState.DayTime;
        }
        else if(Vars.UserData.CurIngameHour <= 24)
        {
            if (curTimeState != TimeState.NightTime)
            {
                EventBus<TimeState>.Publish(TimeState.NightTime);
            }
            curTimeState = TimeState.NightTime;
        }
    }

    public static void TimeUp(int minute, int hour=0)
    //시간보다는 분을 더 자주쓸거같아서 hour은 디폴트 매개변수로 두었습니당!
    {
        Vars.UserData.CurIngameHour += hour;
        Vars.UserData.CurIngameMinute += minute;
        while(Vars.UserData.CurIngameMinute >= Vars.maxIngameMinute)
        {
            Vars.UserData.CurIngameMinute -= Vars.maxIngameMinute;
            Vars.UserData.CurIngameHour++;
        }
        while(Vars.UserData.CurIngameHour >= Vars.maxIngameHour)
        {
            Vars.UserData.CurIngameHour -= Vars.maxIngameHour;
            DateUp();
        }
        TimeStateChange();
    }
    public static void DateUp()
    {
        Vars.UserData.date++;
        if(Vars.UserData.date % 15 == 0)
        {
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon);
        }

        if(Vars.UserData.date > 2)
        {
            EventBus<DateEvent>.Publish(DateEvent.WitchEffect);
        }
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
