using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public RectTransform sliderRect;
    public Slider laternSlider;
    public CampManager campManager;
    public void Start()
    {
        ChangeableStaminaChange();
        ConsumeManager.init();
    }
    void Update()
    {
        slider.value = (float)Vars.UserData.uData.CurStamina / (float)Vars.maxStamina;
    }
    private void ChangeableStaminaChange()
    {
        Vector3 temp = sliderRect.localScale;
        var changeValue = ((float)Vars.UserData.uData.ChangeableMaxStamina / (float)Vars.maxStamina);
        temp.x = changeValue;
        sliderRect.localScale = temp;
    }

    public void OnGUI()
    {
        if (GUILayout.Button("RecoverTiredness"))
        {
            ConsumeManager.RecoveryTiredness();
        }
        if (GUILayout.Button("GettingTired"))
        {
            ConsumeManager.GettingTired(5);
            Debug.Log($"CurStamina {Vars.UserData.uData.CurStamina}");
        }
        if (GUILayout.Button("eat Food"))
        {
            ConsumeManager.RecoveryHunger(5); //최대치 증가 
            ChangeableStaminaChange();
        }
        if (GUILayout.Button("Im hungry"))
        {
            ConsumeManager.GetthingHunger(5); //최대치 감소
            ChangeableStaminaChange();
        }
        if (GUILayout.Button("BlightUp"))
        {
            if (ConsumeManager.CurLanternState != LanternState.Level4)
            {
                ConsumeManager.CurLanternState++;
            }
        }
        if (GUILayout.Button("BlightDown"))
        {
            if (ConsumeManager.CurLanternState != LanternState.None)
            {
                ConsumeManager.CurLanternState--;
            }
        }
        if (GUILayout.Button("OilUp"))
        {
            ConsumeManager.FullingLantern(1);
        }
        if (GUILayout.Button("OilDown"))
        {
            ConsumeManager.ConsumeLantern(1);
        }
        if (GUILayout.Button("BlueMoonObj_OnOff"))
        {
            campManager.OnOffBluemoonObject();
        }
        if (GUILayout.Button("DayNightChange"))
        {
            ConsumeManager.TimeUp(0, 13);
        }
        if (GUILayout.Button("BonFireUp"))
        {
            ConsumeManager.RecoveryBonFire(0, 1);
        }
        if (GUILayout.Button("BonFireDown"))
        {
            ConsumeManager.ConsumeBonfireTime(0, 1);
        }
    }
}
