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
        // �÷��̾ �̵��� �� ���� ȣ�� �Ǿ�� �ϴ� �޼���
        var rnd = Random.Range(0f, 1f);
        if (rnd < escapePercent * 0.01f)
        {
            Debug.Log($"{rnd} ���� Ȯ��: {escapePercent * 0.01f} ���� ���� ����");

            resultPopUp.SetActive(true);
            var tm = resultPopUp.transform.GetChild(0).GetComponent<TMP_Text>();
            tm.text = "The Animal Run Away";
            
            StartCoroutine(Utility.CoSceneChange("2ENO_RandomMap", 3f));
        }
        else
        {
            Debug.Log($"{rnd} ���� Ȯ��: {escapePercent * 0.01f} ���� ���� ����");
        }
    }
    public void EscapingPercentageUp()
    {
        escapePercent += 10;

        IconColor();

        if (escapePercent > 100) // ������.. 100 ���� ���� ������?
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
