using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public MeshRenderer icon;
    public GameObject resultPopUp;
    private int escapePercent;
    public int EscapePercent => escapePercent;

    private void Awake()
    {
        icon.gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 2f, 0f);
        icon.material.color = Color.green;
    }

    public void Escaping()
    {
        // 플레이어가 이동할 때 마다 호출 되어야 하는 메서드
        var rnd = Random.Range(0f, 1f);
        if (rnd < escapePercent * 0.01f)
        {
            Debug.Log($"{rnd} 현재 확률: {escapePercent * 0.01f} 동물 도망 성공");

            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";
            
            StartCoroutine(Utility.CoSceneChange("2ENO_RandomMap", 3f));
        }
        else
        {
            Debug.Log($"{rnd} 현재 확률: {escapePercent * 0.01f} 동물 도망 실패");
        }
    }
    public void EscapingPercentageUp()
    {
        escapePercent += 10;

        IconColor();

        if (escapePercent > 100) // 보정용.. 100 넘을 일이 없을듯?
            escapePercent = 100;
    }

    private void IconColor()
    {
        if (escapePercent < 15)
        {
            icon.material.color = Color.green;
        }
        else if (escapePercent < 35)
        {
            icon.material.color = Color.yellow;
        }
        else if (escapePercent < 55)
        {
            icon.material.color = new Color(255f / 255f, 110f / 255f, 0f);
        }
        else
        {
            icon.material.color = Color.red;
        }
    }
}
