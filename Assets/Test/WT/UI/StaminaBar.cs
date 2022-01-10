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
        slider.value = (float)Vars.UserData.CurStamina / (float)Vars.maxStamina;
    }
    private void ChangeableStaminaChange()
    {
        Vector3 temp = sliderRect.localScale;
        var changeValue = ((float)Vars.UserData.ChangeableMaxStamina / (float)Vars.maxStamina);
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
            Debug.Log($"CurStamina {Vars.UserData.CurStamina}");
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
    }
}
