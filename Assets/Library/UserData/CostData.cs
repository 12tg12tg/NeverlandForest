public class CostData
{
    public int ChangeableMaxStamina { get => Vars.maxStamina - Hunger; set { } }
    public int Hunger { get; set; } = 70;
    public int CurIngameHour { get; set; } = 0;
    public int CurIngameMinute { get; set; } = 0;
    public int LanternCount { get; set; } = 18;
    public int Date { get; set; } = 0;
    public float Tiredness { get; set; } = 100f;
    public float CurStamina { get => Tiredness; set { } }
    public float HunterHp { get; set; } = 100f;
    public float HerbalistHp { get; set; } = 100f;
}
