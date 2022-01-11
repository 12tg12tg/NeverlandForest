using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public void HuntView()
    {
        var coTrans = Utility.CoTranslate(transform, transform.position, new Vector3(0f, 17f, -14.5f), 1f);
        var coRot = Utility.CoRotate(transform, transform.rotation, Quaternion.Euler(new Vector3(55f, 0f, 0f)), 1f);
        StartCoroutine(coTrans);
        StartCoroutine(coRot);
    }
}
