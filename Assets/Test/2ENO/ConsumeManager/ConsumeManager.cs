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
    public static void Init()
    {
        TimeStateChange();
        LanternStateChange();
    }
    public static void SaveConsumableData()
    {
        SaveLoadManager.Instance.Save(SaveLoadSystem.SaveType.ConsumableData);
    }

    public static void GetthingHunger(float hungercount)
    {
        Vars.UserData.uData.Hunger += hungercount;
        if (Vars.UserData.uData.Hunger > 100)
        {
            Vars.UserData.uData.Hunger = 100;
            ChangeableMaxStaminChange();
        }
    }
    public static void RecoveryHunger(float hungercount)
    {
        Vars.UserData.uData.Hunger -= hungercount;
        if (Vars.UserData.uData.Hunger < 0)
        {
            Vars.UserData.uData.Hunger = 0;
        }
        ChangeableMaxStaminChange();
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
    }
    public static void RecoveryTiredness(float recoverTired)
    {
        Vars.UserData.uData.Tiredness += recoverTired;
        if (Vars.UserData.uData.Tiredness > 100)
        {
            Vars.UserData.uData.Tiredness = 100;
        }
    }

    public static void GettingTired(float gettingTired)
    {
        Vars.UserData.uData.Tiredness -= gettingTired;
        GetthingHunger(gettingTired);

        if (Vars.UserData.uData.Tiredness < 0)
        {
            Vars.UserData.uData.Tiredness = 0;
            GameManager.Manager.GameOver(GameOverType.StaminaZero);
        }
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
        Vars.UserData.uData.Hp += recoveryAmount;
        if (Vars.UserData.uData.Hp > Vars.maxHp)
        {
            Vars.UserData.uData.Hp = Vars.maxHp;
        }
    }
    public static bool GetDamage(float damage)
    {
        Vars.UserData.uData.Hp -= damage;
        if (Vars.UserData.uData.Hp <= 0)
        {
            Vars.UserData.uData.Hp = 0;
            return true;
        }
        return false;
    }

    public static void ConsumeLantern(int oil)
    {
        Vars.UserData.uData.LanternCount -= oil;
        if (Vars.UserData.uData.LanternCount < 1)
        {
            Vars.UserData.uData.LanternCount = 1;
        }
        LanternStateChange();
    }
    public static void FullingLantern(int oil)
    {
        SoundManager.Instance.Play(SoundType.Se_OilFulling);

        Vars.UserData.uData.LanternCount += oil;
        if (Vars.UserData.uData.LanternCount > Vars.lanternMaxCount)
        {
            Vars.UserData.uData.LanternCount = Vars.lanternMaxCount;
        }
        LanternStateChange();
    }
    private static void LanternStateChange()
    {
        var count = Vars.UserData.uData.LanternCount;
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
        if (count <= 18 && count > 15)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 15 && count > 11)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 11 && count > 6)
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 6 && count > 0)
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
        if (count <= 18 && count > 15)
        {
            curLanternState = LanternState.Level4;
        }
        else if (count <= 15 && count > 11)
        {
            curLanternState = LanternState.Level3;
        }
        else if (count <= 11 && count > 6)
        {
            curLanternState = LanternState.Level2;
        }
        else if (count <= 6 && count > 0)
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
    }
    public static void RecoveryBonFire(float minute, float hour = 0)
    {
        var totalTime = Vars.UserData.uData.BonfireHour * 60;
        totalTime += minute;
        totalTime += 60 * hour;
        Vars.UserData.uData.BonfireHour = totalTime / 60;
    }
    public static void CostDataReset()
    {
        Vars.UserData.uData.Hunger = 0;
        Vars.UserData.uData.CurIngameHour = 0;
        Vars.UserData.uData.CurIngameMinute = 0;
        Vars.UserData.uData.LanternCount = Vars.lanternMaxCount;
        Vars.UserData.uData.Date = 0;
        Vars.UserData.uData.Tiredness = 100;
        Vars.UserData.uData.Hp = Vars.maxHp;
        Vars.UserData.uData.BonfireHour = 3f;
        SaveConsumableData();
    }
}
