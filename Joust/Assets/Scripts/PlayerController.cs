using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    int direction;
    public float moveSpeed;
    float originalSpeed;
    public float jumpVel;
    Camera camera;
    public GameObject pig;
    public GameObject chicken;

    public GameObject hunterEgg;
    public GameObject shadowLordEgg;

    public GameObject seagullBody;
    public GameObject roosterBody;
    public GameObject cowBody;

    public AudioClip flap1;
    public AudioClip flap2;
    public AudioClip flap3;
    public AudioClip hitWall;
    public AudioClip enemyDie;
    public AudioClip getEgg;
    public AudioClip roosterDeath;
    public AudioClip seagullDeath;
    public AudioClip cowDeath;

    AudioSource flap1Source;
    AudioSource flap2Source;
    AudioSource flap3Source;
    AudioSource hitWallSource;
    AudioSource enemyDieSource;
    AudioSource getEggSource;
    AudioSource roosterDeathSource;
    AudioSource seagullDeathSource;
    AudioSource cowDeathSource;
    int oldWave;

    float time = 0.5f;
    float invinsiblityTime;
    public float spawnProtectionLimit;
    float timeBetweenKills;
    bool offMap;

    float respawnTimer;

    public Animator anim;

    enum PlayerState
    {
        Idle,
        Knocking,
        Respawning
    }

    PlayerState currentState;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        direction = 1;
        camera = Camera.main;
        flap1Source = AddAudio(false, false, 0.5f);
        flap2Source = AddAudio(false, false, 0.5f);
        flap3Source = AddAudio(false, false, 0.5f);
        hitWallSource = AddAudio(false, false, 0.5f);
        enemyDieSource = AddAudio(false, false, 0.5f);
        getEggSource = AddAudio(false, false, 0.5f);
        roosterDeathSource = AddAudio(false, false, 0.5f);
        seagullDeathSource = AddAudio(false, false, 0.5f);
        cowDeathSource = AddAudio(false, false, 0.5f);
        invinsiblityTime = 0;
        timeBetweenKills = 0;
        offMap = false;
        currentState = PlayerState.Respawning;
        originalSpeed = moveSpeed;
        int oldWave = EnemySpawnerController.instance.GetWave();
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
        if (oldWave != EnemySpawnerController.instance.GetWave()) {
            oldWave = EnemySpawnerController.instance.GetWave();
            moveSpeed = originalSpeed * Mathf.Pow(1.01f, EnemySpawnerController.instance.GetWave());
        }
        if (respawnTimer > 1f)
        {
            currentState = PlayerState.Idle;
        }
        else
        {
            respawnTimer += Time.deltaTime;
        }
        //Has to be put in Update to work
        //if W is pressed then add a velocity upwards, this way it will always move the same hieht up and will only be activated when pressed
        if (Input.GetKeyDown(KeyCode.W) && currentState == PlayerState.Idle)
        {

            anim.Play("PlayerFlying");
            int randomNum = Random.Range(0, 2);
            if (randomNum == 0)
            {
                flap1Source.clip = flap1;
                flap1Source.Play();
            }
            else if (randomNum == 1)
            {
                flap2Source.clip = flap2;
                flap2Source.Play();
            }
            else if (randomNum == 2)
            {
                flap3Source.clip = flap3;
                flap3Source.Play();
            }
            rb.velocity = new Vector3(0, jumpVel, 0);
        }
        invinsiblityTime += Time.deltaTime;
        timeBetweenKills += Time.deltaTime;
        if (transform.position.x < -13) {
            transform.position = new Vector2(11.2f, transform.position.y);
        }
        if (transform.position.x > 13)
        {
            transform.position = new Vector2(-11.2f, transform.position.y);
        }
    }

    void FixedUpdate()
    {

        //Controls the movement from left to right
        if (Input.GetKey(KeyCode.A) && currentState != PlayerState.Knocking && currentState == PlayerState.Idle)
        {
            transform.position += transform.right * moveSpeed * Time.fixedDeltaTime;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            direction = -1;
        }
        if (Input.GetKey(KeyCode.D) && currentState != PlayerState.Knocking && currentState == PlayerState.Idle)
        {
            transform.position += transform.right * moveSpeed * Time.fixedDeltaTime;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            direction = 1;
        }
        //player is inactive during knockback
        if (currentState == PlayerState.Knocking)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                currentState = PlayerState.Idle;
                time = 0.5f;
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (camera != null)
        {
            offMap = true;
            //this variable represents half of the screens height
            float halfScreenHeight = camera.orthographicSize;
            //this variable represents half of the screens width
            float halfScreenWidth = camera.orthographicSize * camera.aspect;
            //since we have half of the screens width we need to multiply it by 2 to get the full width, but to make the object disapear for longer we can increase this
            float distanceMultiplier = 2.1f;
            //when the object goes off the screen it is moved backwards or forwards to the other side of the sceen
            if (transform.position.y < halfScreenHeight && transform.position.y > -halfScreenHeight)
            {
                transform.position = new Vector3(transform.position.x - ((halfScreenWidth * distanceMultiplier) * direction), transform.position.y);
            }
            else if (transform.position.y > halfScreenHeight)
            {
                transform.position = new Vector2(transform.position.x, halfScreenHeight);
            }
            else if (transform.position.y < -halfScreenHeight)
            {
                transform.position = new Vector2(transform.position.x, -halfScreenHeight);
            }
        }
    }

    private void OnBecameVisible()
    {
        offMap = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invinsiblityTime > spawnProtectionLimit) {
            if (collision.gameObject.tag == "Enemy"  || collision.gameObject.tag == "Cow" && offMap == false)
            {
                float playerY = transform.position.y;
                float enemyY = collision.transform.position.y;
                // boundary of 0.2 to make it easier so it does not bounce off
                if ((playerY - enemyY < -0.2 || playerY - enemyY > 0.2) && !(playerY < enemyY) && timeBetweenKills > 0.25f)
                {
                    if (collision.gameObject.name == "Seagull(Clone)")
                    {
                        seagullDeathSource.clip = seagullDeath;
                        seagullDeathSource.Play();
                    }
                    else if (collision.gameObject.name == "Rooster(Clone)")
                    {
                        roosterDeathSource.clip = roosterDeath;
                        roosterDeathSource.Play();
                    }
                    else if (collision.gameObject.name == "Cow(Clone)") {
                        cowDeathSource.clip = cowDeath;
                        cowDeathSource.Play();
                    }
                    else {
                        enemyDieSource.clip = enemyDie;
                        enemyDieSource.Play();
                    }
                    Destroy(collision.gameObject);

                    if (collision.gameObject.name == "Bounder(Clone)")
                    {
                        Instantiate(chicken, collision.transform.position, collision.transform.rotation);
                        Instantiate(hunterEgg, new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f), transform.rotation);
                    }
                    else if (collision.gameObject.name == "Hunter(Clone)" || collision.gameObject.name == "ShadowLord(Clone)")
                    {
                        Instantiate(chicken, collision.transform.position, collision.transform.rotation);
                        Instantiate(shadowLordEgg, new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f), transform.rotation);
                    }else if(collision.gameObject.name == "Seagull(Clone)")
                    {
                        Instantiate(seagullBody, new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f), transform.rotation);

                    }
                    else if (collision.gameObject.name == "Rooster(Clone)")
                    {
                        Instantiate(roosterBody, new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f), transform.rotation);
                    }
                    else if (collision.gameObject.name == "Cow(Clone)")
                    {
                        Instantiate(cowBody, new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f), transform.rotation);
                    }
                    timeBetweenKills = 0;
                }
                else if ((playerY - enemyY < -0.2 || playerY - enemyY > 0.2) && (playerY < enemyY))
                {
                    UIManager.instance.LostLife();
                    Instantiate(pig, transform.position, transform.rotation);
                    Destroy(this.gameObject);
                }
                else
                {
                    // bounces the player back if on the same level
                    currentState = PlayerState.Knocking;
                    rb.velocity = new Vector3(moveSpeed * -direction * 1.5f, 0, 0);
                    hitWallSource.clip = hitWall;
                    hitWallSource.Play();
                }
            }
            else if (collision.gameObject.tag == "Egg")
            {
                getEggSource.clip = getEgg;
                getEggSource.Play();
                Destroy(collision.gameObject);
                UIManager.instance.GotEgg();
            }
            else if (collision.gameObject.tag == "Ground")
            {
                ContactPoint2D contact = collision.GetContact(0);
                float myYPos = transform.position.y;
                float theirYPos = contact.point.y;
                if (myYPos - theirYPos > -0.4 && myYPos - theirYPos < 0.4)
                {
                    hitWallSource.clip = hitWall;
                    hitWallSource.Play();
                }

            }
            else if (collision.gameObject.tag == "Poop")
            {
                Destroy(this.gameObject);
                Instantiate(pig, transform.position, transform.rotation);
                UIManager.instance.LostLife();
            }
        }
    }


}
