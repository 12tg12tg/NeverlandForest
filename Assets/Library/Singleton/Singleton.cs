using UnityEngine;

#region 강사님이 추천해주셨던 것
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            //if (applicationIsQuitting)
            //{
            //    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
            //        "' already destroyed on application quit." +
            //        " Won't create again - returning null.");
            //    return null;
            //}

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }
    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
#endregion

#region 강사님 디자인패턴 수업 때 있던 오픈소스
//public class Singleton<T> : MonoBehaviour where T : Component
//{
//    private static T _instance;

//    public static T Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = FindObjectOfType<T>();

//                if (_instance == null)
//                {
//                    GameObject obj = new GameObject();
//                    obj.name = typeof(T).Name;
//                    _instance = obj.AddComponent<T>();
//                }
//            }

//            return _instance;
//        }
//    }

//    public virtual void Awake()
//    {
//        if (_instance == null)
//        {
//            _instance = this as T;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//}
#endregion