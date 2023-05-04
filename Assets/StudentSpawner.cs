using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentSpawner : MonoBehaviour
{
    public GameObject Student;
    public GameObject targetGameObject;
    public float spawninterval = 20f;
    private bool once = false;

    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawninterval && !once)
        {
            once = true;
            SpawnAndSetTarget();
        }

    }
    void SpawnAndSetTarget()
    {
        Debug.Log("Spawn Student");
        GameObject newObject = Instantiate(Student, transform.position, Quaternion.identity);
        GameObject objective = GameObject.Find("PosterTree");
        Debug.Log("Set stduent to tree");
       // UnityEngine.AI.NavMeshAgent navMeshAgent = newObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
/*        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = true; // stop the agent from moving
        navMeshAgent.updatePosition = true; // enable position updates
        navMeshAgent.updateRotation = true; // enable rotation updates*/
        newObject.GetComponent<RagdollOn>().target = objective;
        newObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
    }
}
