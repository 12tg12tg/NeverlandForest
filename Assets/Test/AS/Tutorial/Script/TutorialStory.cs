using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public struct TutorialStory
{
    public IEnumerator CoTutorialStory(TMP_Text text, UnityAction action)
    {
        var storyTable = DataTableManager.GetTable<TutorialStoryDataTable>().data;
        var datas = storyTable.Where(x => (x.Value as TutorialStoryDataTableElem).index == (int)StoryType.Prologue)
                              .Select(x => x)
                              .ToList();
        for (int i = 0; i < datas.Count; i++)
        {
            var data = (TutorialStoryDataTableElem)datas[i].Value;
            var description = data.description;
            var colorData = data.color;
            var option = data.option;
            var typing = data.typing;
            text.fontSize = typing ? 48 : 36;
            if (colorData.Length > 0)
            {
                ColorUtility.TryParseHtmlString(colorData, out var color);
                text.color = color;
            }
            text.text += option ? @"""" : "";

            for (int j = 0; j < description.Length; j++)
            {
                if (description[j].Equals('n'))
                {
                    text.text += "\n";
                }
                else
                    text.text += description[j];
                if (typing)
                    yield return new WaitForSeconds(0.1f);
            }

            text.text += option ? @"""" : "";

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

        #region 람다 사용 이전 버전
        //for (int i = 0; i < storyTable.ids.Count; i++)
        //{
        //    var data = storyTable.GetData<TutorialStoryDataTableElem>(storyTable.ids[i]);
        //    if ((int)StoryType.Prologue != data.index)
        //        break;
        //    var description = data.description;
        //    var colorData = data.color;
        //    var option = data.option;
        //    var typing = data.typing;
        //    text.fontSize = typing ? 48 : 36;
        //    if (colorData.Length > 0)
        //    {
        //        ColorUtility.TryParseHtmlString(colorData, out var color);
        //        text.color = color;
        //    }

        //    text.text += option ? @"""" : "";

        //    for (int j = 0; j < description.Length; j++)
        //    {
        //        if(description[j].Equals('n'))
        //        {
        //            text.text += "\n";
        //        }
        //        else
        //            text.text += description[j];
        //        if(typing)
        //            yield return new WaitForSeconds(0.1f);
        //    }

        //    text.text += option ? @"""" : "";

        //    yield return new WaitForSeconds(1f);

        //    float time = 1f;
        //    float timer = 0f;

        //    while (timer < time)
        //    {
        //        timer += Time.deltaTime;
        //        var ratio = timer / time;
        //        var curColor = text.color;
        //        curColor.a = Mathf.Lerp(1, 0, ratio);
        //        text.color = curColor;
        //        yield return null;
        //    }

        //    text.text = "";
        //    text.color = Color.white;
        //}
        #endregion
    }
}
