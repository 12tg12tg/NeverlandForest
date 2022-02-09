public class CostData
{
    public int ChangeableMaxStamina { get => Vars.maxStamina - Hunger; set { } }
    public int Hunger { get; set; } = 0;
    public float CurIngameHour { get; set; } = 0;
    public float CurIngameMinute { get; set; } = 0;
    public float LanternCount { get; set; } = 18;

    public LanternState lanternState { get=> ConsumeManager.CurLanternState; set { } }

    public int Date { get; set; } = 0;
    public float Tiredness { get; set; } = 100f;
    public float Hp { get; set; } = 50f;
    public float BonfireHour { get; set; } = 3f;

}
