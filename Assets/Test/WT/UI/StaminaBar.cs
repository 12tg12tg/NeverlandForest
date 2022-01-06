using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public RectTransform sliderRect;
    public void Start()
    {
        /*  Debug.Log($"Vars.UserData.CurStamina{Vars.UserData.CurStamina}");
          Debug.Log($"Vars.maxStamina;{Vars.maxStamina}");
        */
        Vars.UserData.Tiredness = 100;
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
        Debug.Log(changeValue);
        temp.x =changeValue;
        sliderRect.localScale = temp;
        Debug.Log(sliderRect.localScale);
        Debug.Log($"CurStamina {Vars.UserData.CurStamina}");
        Debug.Log($"Hunger {Vars.UserData.Hunger}");
        Debug.Log(slider.value);
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
