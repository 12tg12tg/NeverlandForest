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
    public static IEnumerator CoRotate(Transform transform, Quaternion start, Quaternion end, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.rotation = Quaternion.Lerp(start, end, ratio);
            timer += Time.deltaTime;
            yield return null;
        }
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
    public static IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time, string SceneName, UnityAction action = null)
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
        SceneManager.LoadScene(SceneName);
    }

    public static IEnumerator CoTranslate(RectTransform transform, Vector3 start, Vector3 end, float time, UnityAction action = null)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.localPosition = Vector3.Lerp(start, end, ratio);

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;
        action?.Invoke();
    }
    public static IEnumerator CoTranslate(Transform transform, Vector3 dest, float speed, float minDist, UnityAction action = null)
    {
        var foward = transform.forward;
        while (Vector3.Distance(transform.position, dest) > minDist)
        {
            transform.position += foward * speed * Time.deltaTime;
            yield return null;
        }
        transform.position = dest;
        action?.Invoke();
    }

    public static IEnumerator CoSceneChange(string SceneName, float timer)
    {
        var time = 0f;
        while (timer > time)
        {
            time += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(SceneName);
    }

    public static IEnumerator CoSceneChange(int scene, float timer, UnityAction action)
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
        float wRatio = Screen.width / cs.referenceResolution.x;
        float hRatio = Screen.height / cs.referenceResolution.y;

        float ratio =
            wRatio * (1f - cs.matchWidthOrHeight) +
            hRatio * (cs.matchWidthOrHeight);

        var size = new Vector2();
        size.x = rt.rect.width * ratio;
        size.y = rt.rect.height * ratio;

        return size;
    }

    public static IEnumerator CoTranslateLookFoward(Transform transform, Vector3 start, Vector3 end, float time, UnityAction action = null)
    {
        Debug.Log(transform.gameObject.name, transform.gameObject);
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
}