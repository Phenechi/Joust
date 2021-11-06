using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.tag = "NotActiveSpawner";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.gameObject.tag = "ActiveSpawner";
    }

    private void OnTriggerStay(Collider other)
    {
        this.gameObject.tag = "NotActiveSpawner";
    }
}
