using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTargetFollow : MonoBehaviour
{
    public GameObject target;
    public GameObject rewardCamera;
    private readonly float distance = 10f;

    public void OnEnable()
    {
        rewardCamera.transform.position = target.transform.position + Vector3.right * distance;
        rewardCamera.transform.LookAt(target.transform);
        rewardCamera.transform.position += new Vector3(0f, 1.5f, 0f);
    }
}
