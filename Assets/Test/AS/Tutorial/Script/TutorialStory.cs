using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialStory
{
    public IEnumerator CoText(TMP_Text text)
    {
        var stroyTable = DataTableManager.GetTable<TutorialStoryDataTable>();
        for (int i = 0; i < stroyTable.ids.Count; i++)
        {
            var description = stroyTable.GetData<TutorialStoryDataTableElem>(stroyTable.ids[i]).description;
            text.text = "";
            yield return new WaitForSeconds(0.5f);

            for (int j = 0; j < description.Length; j++)
            {
                text.text += description[j];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
