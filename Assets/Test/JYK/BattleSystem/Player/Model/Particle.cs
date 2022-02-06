using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Particle : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ProjectileTag poolTag;
    public void Init(UnityAction action = null)
    {
        particle.Play();
        StartCoroutine(CoWaitEndReturnPool(action));
    }
    private IEnumerator CoWaitEndReturnPool(UnityAction action)
    {
        yield return new WaitUntil(() => !particle.isPlaying);
        ProjectilePool.Instance.ReturnObject(poolTag, gameObject);
        action?.Invoke();
        Debug.Log("파티클 반환 완료");
    }
}
