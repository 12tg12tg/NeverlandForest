using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Utility
{
    // �Ű����� ������ ���� �̺�Ʈ �޼ҵ� ���, �ڷ�ƾ�� �ݹ����� Ȱ��
    public static UnityAction arg0Event;
    public static UnityAction<object> arg1Event;
    public static UnityAction<object, object> arg2Event;

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

    public static IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time, Action action = null)
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

    public static IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time, string SceneName, Action action = null)
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


    public static IEnumerator CoTranslate(RectTransform transform, Vector3 start, Vector3 end, float time, Action action = null)
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

    public static IEnumerator CoTranslate2(Transform transform, Vector3 start, Vector3 end, float time, Vector3 lookDir, Action action = null)
    {
        Debug.Log(transform.gameObject.name, transform.gameObject);
        float timer = 0f;
        var dir = Vector3.Normalize(end - start);
        arg1Event?.Invoke(true);
        transform.rotation = Quaternion.LookRotation(dir);
        while (timer < time)
        {
            var ratio = timer / time;
            transform.position = Vector3.Lerp(start, end, ratio);

            timer += Time.deltaTime;
            yield return null;
        }
        action?.Invoke();
        arg0Event?.Invoke();
        transform.rotation = Quaternion.Euler(lookDir);
        arg1Event?.Invoke(false);
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
        return list;
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
}