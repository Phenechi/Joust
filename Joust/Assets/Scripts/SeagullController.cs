using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullController : MonoBehaviour
{

    Rigidbody2D rb;
    public float speed;
    public float verticalRange;
    int direction;
    Camera camera;
    public AudioClip roosterDive;
    public AudioClip seagullDive;
    AudioSource roosterDiveSource;
    AudioSource seagullDiveSource;

    public float plungeSpeed;

    
    public float rateOfFire;

    public GameObject poopPrefab;

    float timer;
    float timeToStart;
    public float timeToStartLimit;
    bool canSpawnStuff;
    bool diveSound;
    enum EnemyState
    {
        Idle,
        Activated,
        Grounded
    }

    EnemyState currentState;
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
        timer = rateOfFire;
        timeToStart = 0;
        canSpawnStuff = true;
        roosterDiveSource = AddAudio(false, false, 0.5f);
        seagullDiveSource = AddAudio(false, false, 0.5f);
    }

    public AudioSource AddAudio(bool loop, bool playOnAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playOnAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -11.2)
        {
            transform.position = new Vector2(11.2f, transform.position.y);
        }
        if (transform.position.x > 11.2)
        {
            transform.position = new Vector2(-11.2f, transform.position.y);
        }
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Idle)
        {
            rb.velocity = Vector2.zero;
            //This line makes the object move horizontally straight
            transform.position += transform.right * speed * Time.deltaTime;
            //This line makes the object move up and down in a sin wave pattern
            transform.position += transform.up * Mathf.Sin(10 * Time.time) * Time.deltaTime * verticalRange;
        }
        else if (currentState == EnemyState.Activated)
        {
            //Finds the player and sends the direction to another method
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (((player.transform.position.x < transform.position.x) && direction == 1) || ((player.transform.position.x > transform.position.x) && direction == -1))
            {
                turnAround();
            }
            Vector3 moveDirection = ((player.transform.position + new Vector3(0, 0.5f, 0)) - this.transform.position).normalized;
            if (diveSound == true) {
                if (this.gameObject.name == "Seagull(Clone)")
                {
                    seagullDiveSource.clip = seagullDive;
                    seagullDiveSource.Play();
                }
                else if (this.gameObject.name == "Rooster(Clone)")
                {
                    roosterDiveSource.clip = roosterDive;
                    roosterDiveSource.Play();
                }
                diveSound = false;
            }
            MoveToTarget(moveDirection);
        }

        //print(poop);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {

            dropPoop();
            timer = rateOfFire;
        }

        timeToStart += Time.deltaTime;
        if (timeToStart < timeToStartLimit)
        {
            this.gameObject.tag = "NotStartedEnemy";
        }
        else
        {
            this.gameObject.tag = "Enemy";
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            ContactPoint2D contact = collision.GetContact(0);
            float myYPos = transform.position.y;
            float theirYPos = contact.point.y;
            if (myYPos - theirYPos > -0.25 && myYPos - theirYPos < 0.25)
            {
                turnAround();
            }
        }
        else
        {
            turnAround();
            transform.position = new Vector3(transform.position.x + (0.5f * direction), transform.position.y, transform.position.z);
        }
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

    private void OnBecameInvisible()
    {
        if (camera != null)
        {
            canSpawnStuff = false;
            //this variable represents half of the screens width
            float halfScreenWidth = camera.orthographicSize * camera.aspect;
            //since we have half of the screens width we need to multiply it by 2 to get the full width, but to make the object disapear for longer we can increase this
            float distanceMultiplier = 2.1f;
            //when the object goes off the screen it is moved backwards or forwards to the other side of the sceen
            transform.position = new Vector3(transform.position.x - (halfScreenWidth * distanceMultiplier * direction), transform.position.y);

        }
    }

    public void OnBecameVisible()
    {
        canSpawnStuff = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            diveSound = true;
            currentState = EnemyState.Activated;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            currentState = EnemyState.Idle;
        }

    }

    void MoveToTarget(Vector3 direction)
    {
        //Sets a velocity towards the player
        rb.AddForce(direction * 0.05f * plungeSpeed, ForceMode2D.Impulse);
    }

    private void OnDestroy()
    {
        UIManager.instance.killedSeagullOrRooster();
    }


    void dropPoop()
    {
        if (canSpawnStuff) {
            Instantiate(poopPrefab, new Vector2(transform.position.x, transform.position.y - 0.5f), transform.rotation);
        }
    }

}
