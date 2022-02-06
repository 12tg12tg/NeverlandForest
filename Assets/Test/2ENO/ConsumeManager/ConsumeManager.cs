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
    Level1,
    Level2,
    Level3,
    Level4,
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
        Debug.Log($"{Vars.UserData.uData.LanternCount}");
        Debug.Log($"{Vars.UserData.uData.lanternState}");
    }
    public static void SaveConsumableData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void LoadConsumableData()
    {
        SaveLoadManager.Instance.Load(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void GetthingHunger(int hungercount)
    {
        if (Vars.UserData.uData.ChangeableMaxStamina > 0)
        {
            Vars.UserData.uData.Hunger += hungercount;
            ChangeableMaxStaminChange();
        }
        SaveConsumableData();
    }
    public static void RecoveryHunger(int hungercount)
    {
        Vars.UserData.uData.Hunger -= hungercount;
        if (Vars.UserData.uData.Hunger < 0)
        {
            Vars.UserData.uData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
        SaveConsumableData();
    }
    public static void RecoveryTiredness()
    {
        var time = CampManager.Instance.RecoverySleepTime;
        var rimittime = Vars.UserData.uData.BonfireHour * 60;
        if (rimittime >= time)
        {
            if (Vars.UserData.uData.Tiredness < Vars.UserData.uData.ChangeableMaxStamina)
            {
                var recoverValue = Vars.UserData.uData.ChangeableMaxStamina - Vars.UserData.uData.Tiredness;
                var afterValue = Vars.UserData.uData.ChangeableMaxStamina / 10;
                var finalValue = time / 30 * afterValue;
                if (Vars.UserData.uData.ChangeableMaxStamina > Vars.UserData.uData.Tiredness)
                {
                    TimeUp(time);
                    Debug.Log($"finalvalue{time}");
                }
                Vars.UserData.uData.Tiredness += finalValue;
                if (Vars.UserData.uData.Tiredness > Vars.UserData.uData.ChangeableMaxStamina)
                {
                    Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina;
                }
            }
            rimittime -= time;
            Vars.UserData.uData.BonfireHour = rimittime / 60;
        }
        else
        {
            Debug.Log("모닥불의 시간이 부족합니다");
        }
        SaveConsumableData();
    }
    public static void RecoveryTiredness(float recoverTired)
    {
        Vars.UserData.uData.Tiredness += recoverTired;
        if (Vars.UserData.uData.Tiredness > 100)
        {
            Vars.UserData.uData.Tiredness = 100;
        }
        SaveConsumableData();

    }

    public static void GettingTired(float gettingTired)
    {
        Vars.UserData.uData.Tiredness -= gettingTired;
        if (Vars.UserData.uData.Tiredness < 0)
        {
            Vars.UserData.uData.Tiredness = 0;
            EventBus<LivingState>.Publish(LivingState.GameOver);
        }
        SaveConsumableData();

    }
    private static void ChangeableMaxStaminChange()
    {
        Vars.UserData.uData.ChangeableMaxStamina = Vars.maxStamina - Vars.UserData.uData.Hunger;
    }
    private static void TirednessChange()
    {
        Vars.UserData.uData.Tiredness = Vars.UserData.uData.ChangeableMaxStamina;
    }
   
    public static void RecoverHp(float recoveryAmount)
    {
        Vars.UserData.uData.HunterHp += recoveryAmount;
        if (Vars.UserData.uData.HunterHp > Vars.hunterMaxHp)
        {
            Vars.UserData.uData.HunterHp = Vars.hunterMaxHp;
        }
        SaveConsumableData();
    }
    public static void GetDamage(float damage)
    {
        Vars.UserData.uData.HunterHp -= damage;
        if (Vars.UserData.uData.HunterHp < 0)
        {
            Vars.UserData.uData.HunterHp = 0;
            EventBus<LivingState>.Publish(LivingState.GameOver);
            CostDataReset();
        }
        SaveConsumableData();
    }

    public static void ConsumeLantern(int oil)
    {
        Vars.UserData.uData.LanternCount -= oil;
        if (Vars.UserData.uData.LanternCount <0)
        {
            Vars.UserData.uData.LanternCount = 0;
            LanternStateChange();
        }
        SaveConsumableData();
    }
    public static void FullingLantern(int oil)
    {
        Vars.UserData.uData.LanternCount += oil;
        if (Vars.UserData.uData.LanternCount > 17)
        {
            Vars.UserData.uData.LanternCount = 17;
            LanternStateChange();
        }
        SaveConsumableData();
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
        SaveConsumableData();
    }

    private static void SetDayLaternState(float count)
    {
        if (count <= 17 && count > 14)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 14 && count > 10)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 10 && count > 5)
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 5 && count > 0)
        {
            curLanternState = LanternState.Level2;
        }
        else if (count == 0)
        {
            curLanternState = LanternState.Level1;
        }
        SaveConsumableData();
    }
    private static void SetNightLaternState(float count)
    {
        if (count <= 17 && count > 14)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 14 && count > 10)
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 10 && count > 5)
        {
            curLanternState = LanternState.Level2;
        }
        else if (count <= 5 && count > 0)
        {
            curLanternState = LanternState.Level1;
        }
        else if (count == 0)
        {
            curLanternState = LanternState.None;
        }
        SaveConsumableData();
    }
    private static void TimeStateChange()
    {
        if (Vars.UserData.uData.CurIngameHour <= 12)
        {
            if (curTimeState != TimeState.DayTime)
            {
                EventBus<TimeState>.Publish(TimeState.DayTime);
            }
            curTimeState = TimeState.DayTime;
        }
        else if (Vars.UserData.uData.CurIngameHour <= 24)
        {
            if (curTimeState != TimeState.NightTime)
            {
                EventBus<TimeState>.Publish(TimeState.NightTime);
            }
            curTimeState = TimeState.NightTime;
        }
        SaveConsumableData();
    }

    public static void TimeUp(float minute, float hour = 0)
    {
        Vars.UserData.uData.CurIngameHour += hour;
        Vars.UserData.uData.CurIngameMinute += minute;
        float consumeTotalMinute = 60 * hour + minute;
        float consumeStamina = consumeTotalMinute / 30;
        GettingTired(consumeStamina);
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
        SaveConsumableData();
    }
    public static void RecoveryTimeUp(int minute, int hour = 0)
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
        SaveConsumableData();
    }
    public static void DateUp()
    {
        Vars.UserData.uData.Date++;
        if (Vars.UserData.uData.Date % 15 == 0)
        {
            EventBus<DateEvent>.Publish(DateEvent.BlueMoon);
        }

        if (Vars.UserData.uData.Date > 2)
        {
            EventBus<DateEvent>.Publish(DateEvent.WitchEffect);
        }
        SaveConsumableData();
    }
    public static void ConsumeBonfireTime(float minute, float hour = 0)
    {
        var totalTime = Vars.UserData.uData.BonfireHour * 60;
        totalTime -= minute;
        totalTime -= 60 * hour;
        if (totalTime < 0)
        {
            totalTime = 0;
        }
        Vars.UserData.uData.BonfireHour = totalTime / 60;
        SaveConsumableData();
    }
    public static void RecoveryBonFire(float minute, float hour = 0)
    {
        var totalTime = Vars.UserData.uData.BonfireHour * 60;
        totalTime += minute;
        totalTime += 60 * hour;
        Vars.UserData.uData.BonfireHour = totalTime / 60;
        SaveConsumableData();
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
        Vars.UserData.uData.BonfireHour = 3f;
    }
}
