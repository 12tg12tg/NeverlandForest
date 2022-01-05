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


public static class ConsumeManager
{
    private static TimeState curTimeState = TimeState.None;
    public static TimeState CurTimeState
    {
        set => curTimeState = value;
        get => curTimeState;
    }
    public static void init()
    {
        StaminaStateChange();
        TimeStateChange();
    }
    public static void SaveConsumableData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void LoadConsumableData()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void IncreaseSatiety(int hungercount) //포만감을 증가
    {   
        Vars.UserData.hunger += hungercount;

        Vars.UserData.maxStamina += hungercount;
        if (Vars.UserData.maxStamina >100)
        {
            Vars.UserData.maxStamina = 100;
        }
        if (Vars.UserData.hunger > 40)
        {
            //40은 임시로 정해둔 수치
            Vars.UserData.hunger = 40;
        }
    }
    public static void DiscreaseSatiety(int hungercount) //포만감을 감소
    {
        Vars.UserData.hunger -= hungercount;
        Vars.UserData.maxStamina -= hungercount;
        if (Vars.UserData.hunger <0)
        {
            Vars.UserData.hunger = 0;
            if (Vars.UserData.maxStamina < 60)
            {
                Vars.UserData.maxStamina = 60;
            }
        }
    }
    public static void  RecoverTiredness(int recoverTired) //피로도 회복
    {
        Vars.UserData.tiredness += recoverTired;
        if (Vars.UserData.tiredness > 40)
        {
            Vars.UserData.tiredness = 40;
        }
    }
    public static void GettingTired(int gettingTired) //피로도 증가
    {
        Vars.UserData.tiredness -= gettingTired;
        if (Vars.UserData.tiredness < 0)
        {
            Vars.UserData.tiredness = 0;
        }
    }
    public static void ReduceBaseStamina(int stamina) //기본스태미나 감소
    {
        Vars.UserData.baseStamina -= stamina;
    }
    public static void IncreaseBaseStamina(int stamina) //기본스태미나 회복
    {
        Vars.UserData.baseStamina -= stamina;
    }
    private static void StaminaStateChange()  //휴식을 취할때만 
        //필요한것 , 소비되는 시간에 따라서 회복량이 달라진다고 했으니깐 매개변수로 시간을 받아야함.
    {   

        /*
        if (curStamina ==0)
        {
            //eventbus에 게임오버를 보내주자.
            //eventbus에 gamestate는 어디서 만들지? gameManger에 만들어져 있던가?
            EventBus<GameState>.Publish(GameState.GameOver);
        }
        */
    }

    private static void TimeStateChange()
    {   
        //현재 인호가 구현해둔 방식은 원 하나에 24시간을 들고있나봄.
        if(Vars.UserData.curIngameHour <= 12)
        {
            if(curTimeState != TimeState.DayTime)
            {
                EventBus<TimeState>.Publish(TimeState.DayTime);
            }
            curTimeState = TimeState.DayTime;
        }
        else if(Vars.UserData.curIngameHour <= 24)
        {
            if (curTimeState != TimeState.NightTime)
            {
                EventBus<TimeState>.Publish(TimeState.NightTime);
            }
            curTimeState = TimeState.NightTime;
        }
    }

    public static void TimeUp(int hour, int minute)
    {
        Vars.UserData.curIngameHour += hour;
        Vars.UserData.curIngameMinute += minute;

        while(Vars.UserData.curIngameMinute >= Vars.maxIngameMinute)
        {
            Vars.UserData.curIngameMinute -= Vars.maxIngameMinute;
            Vars.UserData.curIngameHour++;
        }

        while(Vars.UserData.curIngameHour >= Vars.maxIngameHour)
        {
            Vars.UserData.curIngameHour -= Vars.maxIngameHour;
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
