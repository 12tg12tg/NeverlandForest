using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Floor"))
        {
            gameObject.SetActive(false);
            EventBus<HuntingEvent>.Publish(HuntingEvent.Hunting);
        }
    }
}
