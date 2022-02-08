using System.Collections;
using TMPro;
using UnityEngine;

public enum MainTutorialStage
{
    Story,
    Battle,
    Move,
    Lanturn,
    Event,
    Stamina,
    Camp,
    Clear
}

public class MainTutorial
{
    public MainTutorialStage MainTutorialStage { get;  set; } = MainTutorialStage.Story;

    public TutorialStory tutorialStory = new TutorialStory();

    public MoveTutorial tutorialMove;
    public GatheringTutorial tutorialGathering;
    public CampTutorial tutorialCamp;
    public void Init()
    {
        // ����� ������ ��������
        MainTutorialStage = MainTutorialStage.Move;
    }

    public void NextMainTutorial()
    {
        if (MainTutorialStage != MainTutorialStage.Clear)
        {
            if (MainTutorialStage == MainTutorialStage.Move)
                MainTutorialStage += 2;
            else
                MainTutorialStage++;
        }
    }

    public IEnumerator CoTutorialStory(TMP_Text text)
    {
        var stroyTable = DataTableManager.GetTable<TutorialStoryDataTable>();
        for (int i = 0; i < stroyTable.ids.Count; i++)
        {
            var description = stroyTable.GetData<TutorialStoryDataTableElem>(stroyTable.ids[i]).description;
            
            for (int j = 0; j < description.Length; j++)
            {
                text.text += description[j];
                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(1f);

            float time = 1f;
            float timer = 0f;

            while (timer < time)
            {
                timer += Time.deltaTime;
                var ratio = timer / time;
                var curColor = text.color;
                curColor.a = Mathf.Lerp(1, 0, ratio);
                text.color = curColor;
                yield return null;
            }

            text.text = "";
            text.color = Color.white;
        }
    }
}
