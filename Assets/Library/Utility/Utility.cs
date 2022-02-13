using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class Utility
{
    public static IEnumerator CoRotate(Transform transform, Quaternion start, Quaternion end, float time, UnityAction action = null)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.rotation = Quaternion.Lerp(start, end, ratio);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = end;
        action?.Invoke();
    }

    public static IEnumerator CoRotateLoop(Transform transform, Quaternion start, Quaternion end, float time, UnityAction action = null, UnityAction action2 = null)
    {
        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + time;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            transform.rotation = Quaternion.Lerp(start, end, ratio);
            yield return null;
        }
        transform.rotation = end;
        action?.Invoke();

        startTime = Time.realtimeSinceStartup;
        endTime = startTime + time;
        while (Time.realtimeSinceStartup < endTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            transform.rotation = Quaternion.Lerp(end, start, ratio);
            yield return null;
        }
        transform.rotation = start;

        action2?.Invoke();
    }


    public static IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time, UnityAction action = null)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.position = Vector3.Lerp(start, end, ratio);

            timer += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
    }
    public static IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time, GameScene scene, UnityAction action = null)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.position = Vector3.Lerp(start, end, ratio);

            timer += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
        GameManager.Manager.LoadScene(scene);
    }

    public static IEnumerator CoTranslate(RectTransform transform, Vector2 start, Vector2 end, float time, UnityAction action = null)
    {
        float startTime = Time.realtimeSinceStartup;
        float destTime = startTime + time;

        while (Time.realtimeSinceStartup < destTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            transform.anchoredPosition = Vector2.Lerp(start, end, ratio);

            yield return null;
        }
        transform.anchoredPosition = end;
        action?.Invoke();
    }
    public static IEnumerator CoTranslate(Transform transform, Vector3 dest, float speed, float minDist, UnityAction action = null, bool isMonster = false)
    {
        MonsterUnit monsterUnit = null;
        if (isMonster)
        {
            monsterUnit = transform.GetComponent<MonsterUnit>();
        }
        var startPos = transform.position;
        var foward = (dest - startPos).normalized;

        while (Vector3.Distance(transform.position, dest) > minDist)
        {
            if ((isMonster && !monsterUnit.isPause) || !isMonster)
            {
                transform.position += foward * speed * Time.deltaTime;
            }
            yield return null;
        }
        transform.position = dest;
        action?.Invoke();
    }

    public static IEnumerator CoSceneChange(GameScene scene, float timer)
    {
        var time = 0f;
        while (timer > time)
        {
            time += Time.deltaTime;
            yield return null;
        }
        GameManager.Manager.LoadScene(scene);
    }
    public static IEnumerator CoSceneChange(int sceneIndex, float timer)
    {
        var time = 0f;
        while (timer > time)
        {
            time += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneIndex);
    }

    public static IEnumerator CoSceneChange(string scene, float timer, UnityAction action)
    {
        var time = 0f;
        while (timer > time)
        {
            time += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
        SceneManager.LoadScene(scene);
    }


    public static List<T> Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            var rnd = random.Next(0, i);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;    
        }
        return list; /*반환하지 않아도 되긴 합니다.*/
    }

    public static void ChildrenDestroy(GameObject gameObject)
    {
        var childList = gameObject.GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != gameObject.transform)
                    UnityEngine.Object.Destroy(childList[i].gameObject);
            }
        }
    }

    public static Vector2 RelativeRectSize(CanvasScaler cs, RectTransform rt)
    {
        var cvs = cs.GetComponent<RectTransform>();
        float wRatio = cvs.rect.width / cs.referenceResolution.x;
        float hRatio = cvs.rect.height / cs.referenceResolution.y;

        Debug.Log($"{Screen.width}, {Screen.height}, {wRatio}, {hRatio}");

        float ratio =
            wRatio * (1f - cs.matchWidthOrHeight) +
            hRatio * (cs.matchWidthOrHeight);

        Debug.Log($"{ratio}, {cs.matchWidthOrHeight}");

        var size = new Vector2();
        size.x = rt.rect.width * ratio;
        size.y = rt.rect.height * ratio;

        return size;
    }

    public static IEnumerator CoTranslateLookFoward(Transform transform, Vector3 start, Vector3 end, float time, UnityAction action = null)
    {
        float timer = 0f;
        end.y = 0;
        var dir = Vector3.Normalize(end - start);
        transform.rotation = Quaternion.LookRotation(dir);
        while (timer < time)
        {
            var ratio = timer / time;
            transform.position = Vector3.Lerp(start, end, ratio);
            timer += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
    }
    public static void DeleteSaveData(SaveDataName typeName)
    {
        var path = Path.Combine(Application.persistentDataPath, typeName.ToString() + ".json");
        if (File.Exists(path))
            File.Delete(path);
    }

    public static IEnumerator CoScaleChange(RectTransform rt, Vector3 destScale, float time, UnityAction action = null)
    {
        float startTime = Time.realtimeSinceStartup;
        float destTime = Time.realtimeSinceStartup + time;

        Vector3 startScale = rt.localScale;
        while (Time.realtimeSinceStartup < destTime)
        {
            var ratio = (Time.realtimeSinceStartup - startTime) / time;
            var lerp = Vector3.Lerp(startScale, destScale, ratio);
            lerp.z = 0f;
            rt.localScale = lerp;
            yield return null;
        }
        rt.localScale = destScale;
        action?.Invoke();
    }

    public static void ChangeLayerIncludeChildren(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            ChangeLayerIncludeChildren(child, layer);
        }
    }

    public static IEnumerator FadeIn(RectTransform img, RectTransform bg, Canvas screen, UnityAction action = null)
    {
        bg.gameObject.SetActive(true);

        // 화면 가로 사이즈 가져오기
        var width = screen.GetComponent<RectTransform>().sizeDelta.x;
        // 연출용 이미지 시작위치 잡아주기
        var startPos = img.anchoredPosition = new Vector2(width / 2 + img.sizeDelta.x, 0f);
        // 끝 위치 잡아주기
        var endPos = new Vector2(-width / 2 - 5f, 0f);

        // 고정된 시간
        var time = 1f;
        var timer = 0f;

        while (timer < time)
        {
            var ratio = timer / time;
            img.anchoredPosition = Vector2.Lerp(startPos, -endPos, ratio);
            timer += Time.deltaTime;
            yield return null;
        }

        action?.Invoke();
    }

    public static IEnumerator FadeOut(RectTransform img, RectTransform bg, Canvas screen, UnityAction action = null)
    {
        // 화면 가로 사이즈 가져오기
        var width = screen.GetComponent<RectTransform>().sizeDelta.x;
        // 연출용 이미지 시작위치 잡아주기
        var startPos = img.anchoredPosition = new Vector2(-width / 2, 0f);
        // 끝 위치 잡아주기
        var endPos = new Vector2(img.sizeDelta.x + width / 2 + 5f, 0f);

        // 고정된 시간
        var time = 1f;
        var timer = 0f;

        while (timer < time)
        {
            var ratio = timer / time;
            img.anchoredPosition = Vector2.Lerp(startPos, -endPos, ratio);
            timer += Time.deltaTime;
            yield return null;
        }

        bg.gameObject.SetActive(false);

        action?.Invoke();
    }
}