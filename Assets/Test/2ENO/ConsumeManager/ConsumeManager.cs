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
        if (Vars.UserData.uData.ChangeableMaxStamina >0)
        {
            Vars.UserData.uData.Hunger += hungercount;
            ChangeableMaxStaminChange();
        }
    }
    public static void RecoveryHunger(int hungercount) //���ָ� ���� -> ���¹̳��� �ִ�ġ�� ������
    {
        Vars.UserData.uData.Hunger -= hungercount;
        if (Vars.UserData.uData.Hunger<0)
        {
            Vars.UserData.uData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
    }
    public static void  RecoveryTiredness() //�Ƿε� ȸ�� = ���¹̳ʸ� �ִ�ġ���� ȸ����(����)
    {
        // ȸ���Ҽ� �ִ� �ִ�ġ ���� ȸ�� ��Ű�� �ű⶧���� �Ű������� �����ʿ䰡 ���� �� ����
        // ȸ���� ��ġ�� ����ؼ� �ð��� �Һ��ϴ� �������� ���ߵɰ� ����.
        if (Vars.UserData.uData.CurStamina <Vars.UserData.uData.ChangeableMaxStamina )
        {
            var recoverValue = Vars.UserData.uData.ChangeableMaxStamina - Vars.UserData.uData.Tiredness;
            if (Vars.UserData.uData.ChangeableMaxStamina > Vars.UserData.uData.Tiredness)
            {
                TimeUp((int)recoverValue * 6);
            }
            Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina; // ȸ��
            if (Vars.UserData.uData.Tiredness > Vars.UserData.uData.ChangeableMaxStamina)
            {
                Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina;
            }
            //���¹̳� 10ȸ���� 1�ð� �����̿����ϱ� 1ȸ���� 6���� ����
            CurStaminaChange();
        }
    }
    public static void GettingTired(float gettingTired) //�Ƿε� ����
    {
        Vars.UserData.uData.Tiredness -= gettingTired;
        if (Vars.UserData.uData.Tiredness < 0)
        {
            Vars.UserData.uData.Tiredness = 0;
            //eventbus�� ���ӿ����� ��������.
            //eventbus�� gamestate�� ��� ������? gameManger�� ������� �ִ���?
            EventBus<GameState>.Publish(GameState.GameOver);
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
    private static void FullingLantern(int oil)
    {
        Vars.UserData.uData.LanternCount += oil;
        LanternStateChange();
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
    private static void ConsumeLantern(int oil)
    {   
        // ����ϴ� �����ۿ� ���� oil�� ��ġ���� �����ָ�ɰͰ���.
        Vars.UserData.uData.LanternCount -= oil;
        LanternStateChange();
    }
    private static void LanternStateChange()
    {
        var count = Vars.UserData.uData.LanternCount;
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
    }

    public static void TimeUp(int minute, int hour=0)
    //�ð����ٴ� ���� �� ���־��Ű��Ƽ� hour�� ����Ʈ �Ű������� �ξ����ϴ�!
    {
        Vars.UserData.uData.CurIngameHour += hour;
        Vars.UserData.uData.CurIngameMinute += minute;

        float consumeTotalMinute = 60 * hour + minute;
        float consumeStamina = consumeTotalMinute / 30;
        //30�д� ���¹̳� 1(n)�Һ� �ε� ����ؾ� ����ϰ� ���� �� �� ������
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
