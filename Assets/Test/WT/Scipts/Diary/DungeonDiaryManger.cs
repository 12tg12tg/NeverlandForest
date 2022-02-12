using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonDiaryManger : MonoBehaviour
{
    private static DungeonDiaryManger instance;
    public static DungeonDiaryManger Instacne => instance;

    [Header("�±װ���")]
    public Image inventoryTagImage;
    public Image skillTagImage;
    public Image recipeTagImage;
    public Image notesTagImage;

    [Header("�ǳڰ���")]
    public GameObject inventoryPanel;
    public GameObject skillPanel;
    public GameObject recipePanel;
    public GameObject notesPanel;

    public GameObject bottomui;

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
        gameObject.SetActive(true);
        inventoryPanel.SetActive(true);
        if (SoundManager.Instance.WalkSoundPlayer.isPlaying)
        {
            SoundManager.Instance.PlayWalkSound(false);
        }
        SoundManager.Instance.Play(SoundType.Se_Diary);
    }
    public void OpenSkillPanel()
    {
        AllClose();
        gameObject.SetActive(true);
        skillPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

    }
    public void OpenRecipePanel()
    {
        AllClose();
        gameObject.SetActive(true);
        recipePanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

    }
    public void OpenNotesPanel()
    {
        AllClose();
        gameObject.SetActive(true);
        notesPanel.SetActive(true);
        SoundManager.Instance.Play(SoundType.Se_Diary);

    }

    public void OpenNewBottomUi()
    {
        bottomui.SetActive(true);
    }

}
