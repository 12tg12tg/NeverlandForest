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

public enum StaminaState
{
    None,
    Full,
    SoSo,
    Bad,
    Zero,
}

public static class ConsumeManager
{
    private static int maxStamina = 100;
    private static int curStamina = maxStamina;
    private static StaminaState curStaminaState = StaminaState.None;

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
    public static StaminaState CurStaminaState
    {
        set => curStaminaState = value;
        get => curStaminaState;
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
        EventBus<DateEvent>.Subscribe(DateEvent.BlueMoon, BlueMoon);
        EventBus<DateEvent>.Subscribe(DateEvent.WitchEffect, WitchEffect);

        EventBus<TimeState>.Subscribe(TimeState.DayTime, DayTime);
        EventBus<TimeState>.Subscribe(TimeState.NightTime, NightTime);

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

    private static void StaminaStateChange()
    {
       
    }

    private static void TimeStateChange()
    {
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

    public static void StaminaUp(int staminaValue)
    {
        curStamina += staminaValue;
        if (curStamina > maxStamina)
            curStamina = maxStamina;
        StaminaStateChange();
    }
    // 스테미너가 특정수치 이하가 되었을 때, 한번 메소드 호출 - 변한 상태가 각 개체에 계속 적용?
    public static void StaminaConsume(int staminaValue)
    {
        curStamina -= staminaValue;
        if (curStamina < 0)
            curStamina = 0;
        StaminaStateChange();
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
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon, 54545);
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon,111111111);
        }

        if(date > 2)
        {
            EventBus<DateEvent>.Publish(DateEvent.WitchEffect);
        }
    }
  

    // 이 아래 메소드들은 현재는 테스트 동작!

    private static void StaminaDown(object[] vals)
    {
        if (vals.Length != 1) return;

        if((int)vals[0] <= 0)
        {
            Debug.Log("ConsumeMgr : StaminaDown 실행, 0 이하입니다");
        }
        else if((int)vals[0] <= 25)
        {
            Debug.Log("ConsumeMgr : StaminaDown 실행, 25 이하입니다");
        }
        else if((int)vals[0] <= 50)
        {
            Debug.Log("ConsumeMgr : StaminaDown 실행, 50 이하입니다");
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

        //curStaminaState = StaminaState.None;
        //curStamina = maxStamina;
        //worldTurn = 3;
        //curIngameTime = maxIngameTime;

        EventBus<DateEvent>.Unsubscribe(DateEvent.BlueMoon, BlueMoon);
        EventBus<DateEvent>.Unsubscribe(DateEvent.WitchEffect, WitchEffect);

        EventBus<TimeState>.Unsubscribe(TimeState.DayTime, DayTime);
        EventBus<TimeState>.Unsubscribe(TimeState.NightTime, NightTime);
        EventBus<DateEvent>.ResetEventBus();
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
}

//private ConsumableData consumeData = new ConsumableData();

//public ConsumableData ConsumeData
//{
//    get => consumeData;
//}