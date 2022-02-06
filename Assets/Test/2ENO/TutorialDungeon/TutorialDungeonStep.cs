using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDungeonStep : MonoBehaviour
{
    private static TutorialDungeonStep instance;
    public static TutorialDungeonStep Instance { get => instance; }
    public GameObject tutorialPanel1;
    public GameObject tutorialPanel1_1;
    public GameObject tutorialPanel2;
    private void Awake()
    {
        instance = this;
    }

    public int tutorialStep = 1;
    public int tutorialCount = 0;

    public void NextStep()
    {
        tutorialStep++;
        if(tutorialStep == 2)
            tutorialPanel1.gameObject.SetActive(true);

        if(tutorialStep == 4)
            tutorialPanel1_1.gameObject.SetActive(true);

        if (tutorialStep == 6)
            tutorialPanel2.gameObject.SetActive(true);
    }
}
