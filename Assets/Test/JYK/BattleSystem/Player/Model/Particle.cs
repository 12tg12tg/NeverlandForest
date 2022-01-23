using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ProjectileTag poolTag;
    public void Init()
    {
        particle.Play();
        StartCoroutine(CoWaitEndReturnPool());
    }
    private IEnumerator CoWaitEndReturnPool()
    {
        yield return new WaitUntil(() => !particle.isPlaying);
        ProjectilePool.Instance.ReturnObject(poolTag, gameObject);
        Debug.Log("파티클 반환 완료");
    }
}
