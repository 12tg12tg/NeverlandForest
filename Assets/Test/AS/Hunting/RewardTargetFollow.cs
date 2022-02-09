using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTargetFollow : MonoBehaviour
{
    public GameObject target;
    public GameObject rewardCamera;
    private readonly float distance = 1.5f;

    public void OnEnable()
    {
        rewardCamera.transform.position = target.transform.forward;
        rewardCamera.transform.LookAt(target.transform);
        rewardCamera.transform.position += new Vector3(0f, distance, 0f);
    }
}
