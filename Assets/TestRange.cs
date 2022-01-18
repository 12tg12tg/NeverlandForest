using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRange : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawSphere(transform.position, 3.6f); // 새 날개 포함 범위
    }
}
