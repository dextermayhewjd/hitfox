using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoofBehaviour : MonoBehaviour{
    public float life;
    private float maxLife;
    // Start is called before the first frame update
    void Start(){
        transform.localScale *= Random.Range(0.2f, 1f);
        maxLife = Random.Range(1,3);
        life = maxLife;
    }

    // Update is called once per frame
    void Update(){
        if (life < 0) Destroy(gameObject);
        Vector3 vel = Vector3.up * Mathf.Lerp(0,2,life/maxLife) * Time.deltaTime;
        //vel += Vector3.right * Random.Range(-2,2);
        life-=Time.deltaTime;
        transform.Translate(vel, Space.World);
    }
}
