using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonDiaryManger : MonoBehaviour
{
    private static DungeonDiaryManger instance;
    public static DungeonDiaryManger Instacne => instance;

    [Header("태그관련")]
    public Image inventoryTagImage;
    public Image skillTagImage;
    public Image recipeTagImage;
    public Image notesTagImage;

    [Header("판넬관련")]
    public GameObject inventoryPanel;
    public GameObject skillPanel;
    public GameObject recipePanel;
    public GameObject notesPanel;
    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        OpenInventoryPanel();
    }

    public void AllClose()
    {
        inventoryPanel.SetActive(false);
        skillPanel.SetActive(false);
        recipePanel.SetActive(false);
        notesPanel.SetActive(false);
    }
    public void OpenInventoryPanel()
    {
        AllClose();
        inventoryPanel.SetActive(true);
    }
    public void OpenSkillPanel()
    {
        AllClose();
        skillPanel.SetActive(true);
    }
    public void OpenRecipePanel()
    {
        AllClose();
        recipePanel.SetActive(true);
    }
    public void OpenNotesPanel()
    {
        AllClose();
        notesPanel.SetActive(true);
    }
}
