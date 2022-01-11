using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public RectTransform sliderRect;
    public void Start()
    {
        ChangeableStaminaChange();
    }
    void Update()
    {
        slider.value = (float)Vars.UserData.uData.CurStamina / (float)Vars.maxStamina;
    }
    private void ChangeableStaminaChange()
    {
        Vector3 temp = sliderRect.localScale;
        var changeValue = ((float)Vars.UserData.uData.ChangeableMaxStamina / (float)Vars.maxStamina);
        temp.x =changeValue;
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
            ConsumeManager.RecoveryHunger(5); //�ִ�ġ ���� 
            ChangeableStaminaChange();
        }
        if (GUILayout.Button("Im hungry"))
        {
            ConsumeManager.GetthingHunger(5); //�ִ�ġ ����
            ChangeableStaminaChange();
        }
        if (GUILayout.Button("BlightUp"))
        {
            ConsumeManager.CurLanternState++;
            if (ConsumeManager.CurLanternState ==0)
            {
                ConsumeManager.CurLanternState = LanternState.Level4;
            }
        }
        if (GUILayout.Button("BlightDown"))
        {
            ConsumeManager.CurLanternState--;
            if (ConsumeManager.CurLanternState == LanternState.Level4)
            {
                ConsumeManager.CurLanternState = LanternState.None;
            }
        }
    }
}
