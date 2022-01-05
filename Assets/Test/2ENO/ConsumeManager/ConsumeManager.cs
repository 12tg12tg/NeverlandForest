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

    public static void IncreaseSatiety(int hungercount) //�������� ����
    {   
        Vars.UserData.hunger += hungercount;

        Vars.UserData.maxStamina += hungercount;
        if (Vars.UserData.maxStamina >100)
        {
            Vars.UserData.maxStamina = 100;
        }
        if (Vars.UserData.hunger > 40)
        {
            //40�� �ӽ÷� ���ص� ��ġ
            Vars.UserData.hunger = 40;
        }
    }
    public static void DiscreaseSatiety(int hungercount) //�������� ����
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
    public static void  RecoverTiredness(int recoverTired) //�Ƿε� ȸ��
    {
        Vars.UserData.tiredness += recoverTired;
        if (Vars.UserData.tiredness > 40)
        {
            Vars.UserData.tiredness = 40;
        }
    }
    public static void GettingTired(int gettingTired) //�Ƿε� ����
    {
        Vars.UserData.tiredness -= gettingTired;
        if (Vars.UserData.tiredness < 0)
        {
            Vars.UserData.tiredness = 0;
        }
    }
    public static void ReduceBaseStamina(int stamina) //�⺻���¹̳� ����
    {
        Vars.UserData.baseStamina -= stamina;
    }
    public static void IncreaseBaseStamina(int stamina) //�⺻���¹̳� ȸ��
    {
        Vars.UserData.baseStamina -= stamina;
    }
    private static void StaminaStateChange()  //�޽��� ���Ҷ��� 
        //�ʿ��Ѱ� , �Һ�Ǵ� �ð��� ���� ȸ������ �޶����ٰ� �����ϱ� �Ű������� �ð��� �޾ƾ���.
    {   

        /*
        if (curStamina ==0)
        {
            //eventbus�� ���ӿ����� ��������.
            //eventbus�� gamestate�� ��� ������? gameManger�� ������� �ִ���?
            EventBus<GameState>.Publish(GameState.GameOver);
        }
        */
    }

    private static void TimeStateChange()
    {   
        //���� ��ȣ�� �����ص� ����� �� �ϳ��� 24�ð��� ����ֳ���.
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
