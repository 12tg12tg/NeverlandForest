using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayCheck : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textMesh.text = $"{Vars.UserData.uData.Date + 1}ÀÏÂ÷";
    }
}
