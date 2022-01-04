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
    private static int maxStamina = 100; 
    private static int curStamina = maxStamina; // 배고픔 + 피로도 + 기본 
    private static int baseStamina = 20; //임시의 스태미나 기본수치
    private static int hunger = 40;
    private static int tiredness = 40;

    private static int maxIngameHour = 24;
    private static int curIngameHour = 0;
    private static int maxIngameMinute = 60;
    private static int curIngameMinute = 0;

    private static TimeState curTimeState = TimeState.None;

    private static int date = 1;

    public static int CurStamina
    {
        set => curStamina = value;
        get => curStamina;
    }
  
    public static int CurIngameHour
    {
        set => curIngameHour = value;
        get => curIngameHour;
    }
    public static int CurIngameMinute
    {
        set => curIngameMinute = value;
        get => curIngameMinute;
    }
    public static TimeState CurTimeState
    {
        set => curTimeState = value;
        get => curTimeState;
    }

    public static int Date
    {
        set => date = value;
        get => date;
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
        hunger += hungercount;
        maxStamina += hungercount;
        if (maxStamina>100)
        {
            maxStamina = 100;
        }
        if (hunger > 40)
        {   
            //40은 임시로 정해둔 수치
            hunger = 40;
        }
    }
    public static void DiscreaseSatiety(int hungercount) //포만감을 감소
    {
        hunger -= hungercount;
        maxStamina -= hungercount;
        if (hunger<0)
        {
            hunger = 0;
            if (maxStamina < 60)
            {
                maxStamina = 60;
            }
        }
    }
    public static void  RecoverTiredness(int recoverTired) //피로도 회복
    {
        tiredness += recoverTired;
        if (tiredness > 40)
        {
            tiredness = 40;
        }
    }
    public static void GettingTired(int gettingTired) //피로도 증가
    {
        tiredness -= gettingTired;
        if (tiredness < 0)
        {
            tiredness = 0;
        }
    }
    public static void ReduceBaseStamina(int stamina) //기본스태미나 감소
    {
        baseStamina -= stamina;
    }
    public static void IncreaseBaseStamina(int stamina) //기본스태미나 회복
    {
        baseStamina -= stamina;
    }
    private static void StaminaStateChange()  //휴식을 취할때만 
    {
        curStamina = baseStamina + hunger + tiredness;
        if (curStamina ==0)
        {
            //eventbus에 게임오버를 보내주자.
            //eventbus에 gamestate는 어디서 만들지? gameManger에 만들어져 있던가?
            EventBus<GameState>.Publish(GameState.GameOver);
        }
    }

    private static void TimeStateChange()
    {   
        //현재 인호가 구현해둔 방식은 원 하나에 24시간을 들고있나봄.
        if(curIngameHour <= 12)
        {
            if(curTimeState != TimeState.DayTime)
            {
                EventBus<TimeState>.Publish(TimeState.DayTime);
            }
            curTimeState = TimeState.DayTime;
        }
        else if(curIngameHour <= 24)
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
        curIngameHour += hour;
        curIngameMinute += minute;

        while(curIngameMinute >= maxIngameMinute)
        {
            curIngameMinute -= maxIngameMinute;
            curIngameHour++;
        }

        while(curIngameHour >= maxIngameHour)
        {
            curIngameHour -= maxIngameHour;
            DateUp();
        }

        TimeStateChange();
    }

    public static void DateUp()
    {
        date++;
        if(date % 15 == 0)
        {
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon);
        }

        if(date > 2)
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
