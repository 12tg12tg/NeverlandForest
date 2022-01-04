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
    private static int curStamina = maxStamina; // ����� + �Ƿε� + �⺻ 
    private static int baseStamina = 20; //�ӽ��� ���¹̳� �⺻��ġ
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

    public static void IncreaseSatiety(int hungercount) //�������� ����
    {
        hunger += hungercount;
        maxStamina += hungercount;
        if (maxStamina>100)
        {
            maxStamina = 100;
        }
        if (hunger > 40)
        {   
            //40�� �ӽ÷� ���ص� ��ġ
            hunger = 40;
        }
    }
    public static void DiscreaseSatiety(int hungercount) //�������� ����
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
    public static void  RecoverTiredness(int recoverTired) //�Ƿε� ȸ��
    {
        tiredness += recoverTired;
        if (tiredness > 40)
        {
            tiredness = 40;
        }
    }
    public static void GettingTired(int gettingTired) //�Ƿε� ����
    {
        tiredness -= gettingTired;
        if (tiredness < 0)
        {
            tiredness = 0;
        }
    }
    public static void ReduceBaseStamina(int stamina) //�⺻���¹̳� ����
    {
        baseStamina -= stamina;
    }
    public static void IncreaseBaseStamina(int stamina) //�⺻���¹̳� ȸ��
    {
        baseStamina -= stamina;
    }
    private static void StaminaStateChange()  //�޽��� ���Ҷ��� 
    {
        curStamina = baseStamina + hunger + tiredness;
        if (curStamina ==0)
        {
            //eventbus�� ���ӿ����� ��������.
            //eventbus�� gamestate�� ��� ������? gameManger�� ������� �ִ���?
            EventBus<GameState>.Publish(GameState.GameOver);
        }
    }

    private static void TimeStateChange()
    {   
        //���� ��ȣ�� �����ص� ����� �� �ϳ��� 24�ð��� ����ֳ���.
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
        Debug.Log("ConsumeMgr : ��繮 ����");
    }
    private static void WitchEffect(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : ���� ����Ʈ ����");
    }
    private static void DayTime(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : DayTime ����");
    }
    private static void NightTime(object[] vals)
    {
        if (vals.Length != 0) return;
        Debug.Log("ConsumeMgr : NightTime ����");
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
