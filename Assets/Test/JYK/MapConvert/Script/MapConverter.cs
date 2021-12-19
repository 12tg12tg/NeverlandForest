using System.Collections;
using UnityEngine;

public class MapConverter : MonoBehaviour
{
    public enum Side
    {
        Up, Down, Right, Left
    }

    public Transform diceUpPlane;
    public MapBlock test_CurMap;
    public MapBlock test_DestMap;

    private float activeTime = 0.5f;
    private float maxCameraZoomDistance = 150f;
    private float activeTerm = 0.5f;

    private void OnGUI()
    {
        if (GUILayout.Button("Down"))
        {
            Init(test_CurMap, test_DestMap, Side.Down);
        }
        if (GUILayout.Button("Up"))
        {
            Init(test_CurMap, test_DestMap, Side.Up);
        }
        if (GUILayout.Button("Right"))
        {
            Init(test_CurMap, test_DestMap, Side.Right);
        }
        if (GUILayout.Button("Left"))
        {
            Init(test_CurMap, test_DestMap, Side.Left);
        }
        if (GUILayout.Button("Set initial state"))
        {
            test_CurMap.transform.localPosition = Vector3.zero;
            test_CurMap.transform.rotation = Quaternion.identity;

            test_DestMap.transform.SetParent(null);
            test_DestMap.transform.position = new Vector3(0f, 27.6f, 125f);
            test_DestMap.transform.rotation = Quaternion.identity;
        }
    }

    public void Init(MapBlock curMap, MapBlock destMap, Side side)
    {
        destMap.SetOriginal(curMap.transform);
        destMap.transform.SetParent(transform);
        Bounds squareBound = curMap.floorBlock.bounds;
        float dx = (squareBound.size.x - squareBound.size.y) / 2;

        switch (side)
        {
            case Side.Up:
                destMap.transform.rotation *= Quaternion.Euler(0f, 0f, 90f);
                destMap.transform.Translate(-dx, -dx, 0f, Space.World);
                break;
            case Side.Down:
                destMap.transform.rotation *= Quaternion.Euler(0f, 0f, -90f);
                destMap.transform.Translate(dx, -dx, 0f, Space.World);
                break;
            case Side.Right:
                destMap.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
                destMap.transform.Translate(0f, -dx, dx, Space.World);
                break;
            case Side.Left:
                destMap.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                destMap.transform.Translate(0f, -dx, -dx, Space.World);
                break;
        }

        StartCoroutine(CoMapConvert(curMap, destMap, side));
    }

    private IEnumerator CoMapConvert(MapBlock curMap, MapBlock destMap, Side side)
    {
        var camera = Camera.main.transform;
        var originalCamera = camera.position;
        var topViewPos = originalCamera - camera.forward * maxCameraZoomDistance;
        yield return StartCoroutine(CoTranslate(camera, originalCamera, topViewPos, activeTime));
        yield return new WaitForSeconds(activeTerm);


        var originalRotation = transform.rotation;
        Quaternion destRotation = Quaternion.identity;
        switch (side)
        {
            case Side.Up:
                destRotation = originalRotation * Quaternion.Euler(0f, 0f, -90f);
                break;
            case Side.Down:
                destRotation = originalRotation * Quaternion.Euler(0f, 0f, 90f);
                break;
            case Side.Right:
                destRotation = originalRotation * Quaternion.Euler(-90f, 0f, 0f);
                break;
            case Side.Left:
                destRotation = originalRotation * Quaternion.Euler(90f, 0f, 0f);
                break;
        }
        yield return StartCoroutine(CoRotate(transform, originalRotation, destRotation, activeTime));
        yield return new WaitForSeconds(activeTerm);


        yield return StartCoroutine(CoTranslate(camera, topViewPos, originalCamera, activeTime));
        camera.position = originalCamera;

        transform.rotation = Quaternion.identity;
        destMap.transform.SetParent(diceUpPlane);
        destMap.transform.rotation = Quaternion.identity;
        destMap.transform.localPosition = Vector3.zero; //세팅초기화

        curMap.transform.position = new Vector3(0f, 27.6f, 125f); //기존맵은 풀로 되돌리거나, SetActivate false로.
    }

    private IEnumerator CoRotate(Transform transform, Quaternion start, Quaternion end, float time)
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

    private IEnumerator CoTranslate(Transform transform, Vector3 start, Vector3 end, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            var ratio = timer / time;
            transform.position = Vector3.Lerp(start, end, ratio);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}