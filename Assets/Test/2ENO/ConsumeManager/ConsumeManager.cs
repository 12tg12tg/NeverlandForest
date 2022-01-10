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
    Level1,//���Ⱑ �����ܰ�
    Level2,
    Level3,
    Level4, //���Ⱑ Ǯ
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
        TirednessChange(); //������ �Ƿε��� ChangeableMaxStamina;
    }
    public static void SaveConsumableData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void LoadConsumableData()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void GetthingHunger(int hungercount) //���ָ� ���� -> �ڿ��Һ�� �ؾߵ�.
    {
        if (Vars.UserData.ChangeableMaxStamina >0)
        {
            Vars.UserData.Hunger += hungercount;
            ChangeableMaxStaminChange();
        }
    }
    public static void RecoveryHunger(int hungercount) //���ָ� ���� -> ���¹̳��� �ִ�ġ�� ������
    {
        Vars.UserData.Hunger -= hungercount;
        if (Vars.UserData.Hunger<0)
        {
            Vars.UserData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
    }
    public static void  RecoveryTiredness() //�Ƿε� ȸ�� = ���¹̳ʸ� �ִ�ġ���� ȸ����(����)
    {
        // ȸ���Ҽ� �ִ� �ִ�ġ ���� ȸ�� ��Ű�� �ű⶧���� �Ű������� �����ʿ䰡 ���� �� ����
        // ȸ���� ��ġ�� ����ؼ� �ð��� �Һ��ϴ� �������� ���ߵɰ� ����.
        if (Vars.UserData.CurStamina<Vars.UserData.ChangeableMaxStamina )
        {
            var recoverValue = Vars.UserData.ChangeableMaxStamina - Vars.UserData.Tiredness;
            if (Vars.UserData.ChangeableMaxStamina > Vars.UserData.Tiredness)
            {
                TimeUp(recoverValue * 6);
            }
            Vars.UserData.Tiredness = Vars.UserData.ChangeableMaxStamina; // ȸ��
            if (Vars.UserData.Tiredness > Vars.UserData.ChangeableMaxStamina)
            {
                Vars.UserData.Tiredness = Vars.UserData.ChangeableMaxStamina;
            }
            //���¹̳� 10ȸ���� 1�ð� �����̿����ϱ� 1ȸ���� 6���� ����
            CurStaminaChange();
        }
    }
    public static void GettingTired(int gettingTired) //�Ƿε� ����
    {
        Vars.UserData.Tiredness -= gettingTired;
        if (Vars.UserData.Tiredness < 0)
        {
            Vars.UserData.Tiredness = 0;
            //eventbus�� ���ӿ����� ��������.
            //eventbus�� gamestate�� ��� ������? gameManger�� ������� �ִ���?
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
        // ����ϴ� �����ۿ� ���� oil�� ��ġ���� �����ָ�ɰͰ���.
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
        //���� ��ȣ�� �����ص� ����� �� �ϳ��� 24�ð��� ����ֳ���.
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
    //�ð����ٴ� ���� �� ���־��Ű��Ƽ� hour�� ����Ʈ �Ű������� �ξ����ϴ�!
    {
        Vars.UserData.CurIngameHour += hour;
        Vars.UserData.CurIngameMinute += minute;

        var consumeTotalMinute = 60 * hour + minute;
        var consumeStamina = consumeTotalMinute / 30;
        //var consumeStaminamod = consumeTotalMinute % 30; 
        //30�д� ���¹̳� 1(n)�Һ� �ε� ����ؾ� ����ϰ� ���� �� �� ������
        GettingTired(consumeStamina);
        //GettingTired(consumeStaminamod);

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
