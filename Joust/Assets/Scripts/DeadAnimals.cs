using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAnimals : MonoBehaviour
{
    public float speed;
    public float verticalRange;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
        //This line makes the object move up and down in a sin wave pattern
        transform.position += transform.up * Mathf.Sin(10 * Time.time) * Time.deltaTime * verticalRange;
    }

    private void OnBecameInvisible()
    {
        if (this.gameObject.name == "Pig(Clone)" && UIManager.instance.GetLives() > 0) {
            PlayerRespawnController.instance.Respawn();
        }
        Destroy(this.gameObject);
    }


}
