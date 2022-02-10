using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public struct TutorialStory
{
    public IEnumerator CoTutorialStory(TMP_Text text, UnityAction action)
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
        yield return new WaitForSeconds(1f);

        action?.Invoke();
    }
}
