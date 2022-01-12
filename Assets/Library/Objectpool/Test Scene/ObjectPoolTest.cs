using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ObjectPoolTest : MonoBehaviour
{
    public GameObject prefab;

    public List<Transform> sponZone;
    public Transform goal;
    private ObjectPool<GameObject> agentPool;
    public Material mat;
    public Transform parent;

    private float timer;
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    private void Awake()
    {
        agentPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroyed, false, 10, 15);
    }
    private GameObject OnCreate()
    {
        InfoObjectPool.Instance.capacity++;
        var go = Instantiate(prefab, parent);
        go.GetComponent<NavMeshAgent>().enabled = false;
        go.SetActive(false);
        return go;
    }
    private void OnGet(GameObject go)
    {
        var count = sponZone.Count;
        var rand = Random.Range(0, count);

        go.transform.position = sponZone[rand].position;
        go.GetComponent<NavMeshAgent>().enabled = true;
        go.SetActive(true);
        InfoObjectPool.Instance.count++;
    }
    private void OnRelease(GameObject go)
    {
        go.GetComponent<NavMeshAgent>().enabled = false;
        go.GetComponent<MeshRenderer>().material = mat;
        go.SetActive(false);
        InfoObjectPool.Instance.count--;
    }
    private void OnDestroyed(GameObject go)
    {
        InfoObjectPool.Instance.capacity--;
        Destroy(go);
    }

    private void Update()
    {
        Debug.Log($"{agentPool.CountActive} + {agentPool.CountInactive} = {agentPool.CountAll}");

        if (Input.GetMouseButtonDown(0))
        {
            timer = 0f;
            var newGo = agentPool.Get();
            var agent = newGo.GetComponent<NavMeshAgent>();
            agent.destination = goal.position;
            agents.Add(agent);
        }   
        
        timer += Time.deltaTime;
        if(timer > 5f)
        {
            timer = 0f;
            var newGo = agentPool.Get();
            var agent = newGo.GetComponent<NavMeshAgent>();
            agent.destination = goal.position;
            agents.Add(agent);
        }

        for (int i = 0; i < agents.Count; )
        {
            if (Vector3.Distance(agents[i].transform.position, agents[i].destination) < 2f)
            {
                StartCoroutine(CoUp(agents[i].transform));
                agents.RemoveAt(i);
            }
            else
                i++;
        }
    }

    private IEnumerator CoUp(Transform go)
    {
        go.GetComponent<NavMeshAgent>().enabled = false;
        float timer = 0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;

            var pos = go.position;
            pos.y += 1;
            go.position = pos;
            yield return null;
        }
        agentPool.Release(go.gameObject);
    }
}
