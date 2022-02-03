using System.Collections;
using TMPro;
using UnityEngine;

public enum MainTutorialStage
{
    None,
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
    public MainTutorialStage mainTutorialStage = MainTutorialStage.None;

    public void NextStage(MainTutorialStage stage)
    {
        mainTutorialStage = stage;
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
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1.5f);

            text.text = "";
        }
    }
}
