using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNPC_s : MonoBehaviour
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
        if(timer >= spawninterval && !once)
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
        newObject.GetComponent<RagdollOn>().target = objective;
    }
}
