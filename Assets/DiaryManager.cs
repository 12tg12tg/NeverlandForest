using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiaryManager : MonoBehaviour
{
    public Image infomations;
    public Image skills;
    public Image notes;
    public Image recipes;

    public GameObject infomationPanel;
    public GameObject skillPanel;
    public GameObject notePanel;
    public GameObject recipePanel;


    public void Awake()
    {
        infomations.gameObject.AddComponent<Button>();
        skills.gameObject.AddComponent<Button>();
        notes.gameObject.AddComponent<Button>();
        recipes.gameObject.AddComponent<Button>();

        var infobutton = infomations.gameObject.GetComponent<Button>();
        infobutton.onClick.AddListener(() => OpenInfomation());

        var skillbutton = skills.gameObject.GetComponent<Button>();
        skillbutton.onClick.AddListener(() => OpenSkill());

        var notebutton = notes.gameObject.GetComponent<Button>();
        notebutton.onClick.AddListener(() => OpenNote());

        var recipebutton = recipes.gameObject.GetComponent<Button>();
        recipebutton.onClick.AddListener(() => OpenRecipe());


    }
    public void OpenInfomation()
    {
        infomationPanel.SetActive(true);
        skillPanel.SetActive(false);
        notePanel.SetActive(false);
        recipePanel.SetActive(false);
    }
    public void OpenSkill()
    {
        infomationPanel.SetActive(false);
        skillPanel.SetActive(true);
        notePanel.SetActive(false);
        recipePanel.SetActive(false);
    }

    public void OpenNote()
    {
        infomationPanel.SetActive(false);
        skillPanel.SetActive(false);
        notePanel.SetActive(true);
        recipePanel.SetActive(false);
    }
    public void OpenRecipe()
    {
        infomationPanel.SetActive(false);
        skillPanel.SetActive(false);
        notePanel.SetActive(false);
        recipePanel.SetActive(true);
    }
}
