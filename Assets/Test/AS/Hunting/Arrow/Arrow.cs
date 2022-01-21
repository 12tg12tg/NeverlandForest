using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;
    public GameObject hitArrow;
    public float angle;

    private const float speed = 30f;
    private const float gravity = 9.8f;
    private const float maxDistance = 30f;

    public IEnumerator Shoot(Vector3 targerPos)
    {
        // 높이 계산
        var distance = Vector3.Distance(transform.position, targerPos);
        var velocity = distance / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / gravity);
        var y = Mathf.Sqrt(velocity) * Mathf.Sin(angle * Mathf.Deg2Rad);
        var height = y * distance / maxDistance;

        var time = distance / speed;

        var startArrowPos = transform.position;
        var centerPos = (startArrowPos + targerPos) * 0.5f + new Vector3(0f, height, 0f);

        var timer = 0f;
        while (transform.position.y > 0f)
        {
            timer += Time.deltaTime;
            var ratio = timer / time;

            var u = (1 - ratio);
            var t2 = ratio * ratio;
            var u2 = u * u;

            var pos = startArrowPos * u2 + centerPos * (ratio * u * 2) + targerPos * t2;
            var dir = ratio * targerPos + (u - ratio) * centerPos - u * startArrowPos;

            transform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            hitArrow.SetActive(true);
            EventBus<HuntingEvent>.Publish(HuntingEvent.Hunting);
        }
        else if(other.CompareTag("Floor"))
        {
            EventBus<HuntingEvent>.Publish(HuntingEvent.Hunting);
        }
    }
}
