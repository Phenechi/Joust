using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowScript : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float verticalRange;
    int direction;
    Camera camera;
    public AudioClip cowDive;
    AudioSource cowDiveSource;
    float timeToStart;
    public float timeToStartLimit;
    float timeBetweenStrikes;
    enum EnemyState
    {
        Idle,
        Return,
        Boosted,
        Waiting
    }
    EnemyState currentState;
    Vector3 home;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        direction = 1;
        camera = Camera.main;
        currentState = EnemyState.Idle;
        int randomStartRotation = Random.Range(0, 2);
        if (randomStartRotation == 0)
        {
            turnAround();
        }
        timeBetweenStrikes = 0;
        cowDiveSource = AddAudio(false, false, 0.5f);
    }

    public AudioSource AddAudio(bool loop, bool playOnAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playOnAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    void turnAround()
    {
        currentState = EnemyState.Idle;
        //When the object collides with something, depending on its direction it will face a new direction and continue to move
        direction *= -1;
        if (direction > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EnemyState.Boosted && currentState != EnemyState.Return) {
            if (transform.position.y > -3.15)
            {
                this.GetComponent<CapsuleCollider2D>().enabled = true;
            }
            else if (transform.position.y < -4.3 && transform.position.y > -4.85)
            {
                this.GetComponent<CapsuleCollider2D>().enabled = true;
            }
        }
        timeBetweenStrikes += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (currentState == EnemyState.Idle)
        {
            rb.velocity = Vector2.zero;
            //This line makes the object move horizontally straight
            transform.position += transform.right * speed * Time.deltaTime;
            //This line makes the object move up and down in a sin wave pattern
            transform.position += transform.up * Mathf.Sin(10 * Time.time) * Time.deltaTime * verticalRange;
        }
        else if (currentState == EnemyState.Boosted)
        {
            StartCoroutine(waitForBoosted());
        }
        else if (currentState == EnemyState.Return)
        {
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            Vector3 moveDirection = (home - this.transform.position).normalized;
            MoveToTarget(moveDirection);
            if ((transform.position.x < home.x + 0.03 && transform.position.x > home.x - 0.03) && (transform.position.y < home.y + 0.03 && transform.position.y > home.y - 0.03)) {
                currentState = EnemyState.Idle;

            }
        }
        else if (currentState == EnemyState.Waiting) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 moveDirection = ((player.transform.position + new Vector3(0, 0.1f, 0)) - this.transform.position).normalized;
            MoveToTarget(moveDirection);
        }
        timeToStart += Time.deltaTime;
        if (timeToStart < timeToStartLimit)
        {
            this.gameObject.tag = "CowNotActive";
        }
        else
        {
            this.gameObject.tag = "Cow";
        }
    }

    private void OnBecameInvisible()
    {
        turnAround();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && (transform.position.x < -6.8 || transform.position.x > 6.2) && timeBetweenStrikes > 2 && currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Boosted;
            home = new Vector3(transform.position.x, transform.position.y - 0.035f, transform.position.z);
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            if (((collision.transform.position.x < transform.position.x) && direction == 1) || ((collision.transform.position.x > transform.position.x) && direction == -1))
            {
                turnAround();
            }
            cowDiveSource.clip = cowDive;
            cowDiveSource.Play();
            rb.velocity = ((collision.transform.position + new Vector3(0, 0.1f, 0)) - this.transform.position).normalized * 6.2f;
        }
    }

    void MoveToTarget(Vector3 direction)
    {
        //Sets a velocity towards the player
        rb.velocity = new Vector2(direction.x, direction.y) * speed;
    }

    private IEnumerator waitForBoosted()
    {
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        currentState = EnemyState.Waiting;
        StartCoroutine(waitForKill());
    }

    private IEnumerator waitForKill() {
        yield return new WaitForSeconds(0.8f);
        timeBetweenStrikes = 0;
        currentState = EnemyState.Return;
    }

    private void OnDestroy()
    {
        UIManager.instance.killedShadowLord();
    }
}
