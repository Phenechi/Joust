using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounderController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float verticalRange;
    int direction;
    Camera camera;
    float timeToStart;
    float timeKnocked;
    public float timeToStartLimit;
    public LayerMask groundLayer;
    enum EnemyState
    {
        Idle,
        Activated,
        WaitingForActivation,
        Knocked
    }
    bool hasWaited;
    EnemyState currentState;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        direction = 1;
        camera = Camera.main;
        currentState = EnemyState.Idle;
        int randomStartRotation = Random.Range(0, 2);
        if (randomStartRotation == 0) {
            turnAround();
        }
        timeToStart = 0;
        speed = speed * Mathf.Pow(1.05f, EnemySpawnerController.instance.GetWave());
        this.gameObject.tag = "NotStartedEnemy";
        timeKnocked = 0;
        hasWaited = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -13)
        {
            transform.position = new Vector2(11.2f, transform.position.y);
        }
        if (transform.position.x > 13)
        {
            transform.position = new Vector2(-11.2f, transform.position.y);
        }
    }

    void FixedUpdate()
    {
        if (this.gameObject.tag == "Enemy")
        {
            if (currentState == EnemyState.Idle || currentState == EnemyState.WaitingForActivation)
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
                MoveToTarget(moveDirection);
            }
            else if (currentState == EnemyState.Knocked)
            {
                timeKnocked += Time.deltaTime;
                if (timeKnocked >= 0.2f)
                {
                    currentState = EnemyState.Idle;
                    timeKnocked = 0;
                }
            }
        }
        timeToStart += Time.deltaTime;
        if (timeToStart > timeToStartLimit)
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
            if (myYPos - theirYPos > -0.4 && myYPos - theirYPos < 0.4)
            {
                turnAround();
            }
        }
        else
        {
            transform.position = new Vector2(transform.position.x + (0.1f * -direction), transform.position.y);
            turnAround();
            currentState = EnemyState.Knocked;
            rb.velocity = new Vector3(speed * direction, 0, 0);
        }
    }

    void turnAround()
    {
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
            //this variable represents half of the screens width
            float halfScreenWidth = camera.orthographicSize * camera.aspect;
            //since we have half of the screens width we need to multiply it by 2 to get the full width, but to make the object disapear for longer we can increase this
            float distanceMultiplier = 2.1f;
            //when the object goes off the screen it is moved backwards or forwards to the other side of the sceen
            transform.position = new Vector3(transform.position.x - (halfScreenWidth * distanceMultiplier * direction), transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && this.gameObject.tag == "Enemy")
        {
            bool inBetween = Physics2D.Raycast(transform.position,
            (collision.gameObject.transform.position - this.transform.position).normalized,
                Vector2.Distance(transform.position, collision.gameObject.transform.position),
                    groundLayer);

            Debug.DrawRay(transform.position,
            (-1 * this.transform.position + collision.gameObject.transform.position).normalized *
                Vector2.Distance(transform.position, collision.gameObject.transform.position), Color.red);
            if (!inBetween)
            {
                currentState = EnemyState.WaitingForActivation;
                StartCoroutine(waitToActivate());
                hasWaited = true;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && this.gameObject.tag == "Enemy")
        {
            bool inBetween = Physics2D.Raycast(transform.position,
            (collision.gameObject.transform.position - this.transform.position).normalized,
                Vector2.Distance(transform.position, collision.gameObject.transform.position),
                    groundLayer);
            Debug.DrawRay(transform.position,
            (-1 * this.transform.position + collision.gameObject.transform.position).normalized *
                Vector2.Distance(transform.position, collision.gameObject.transform.position), Color.red);
            if (inBetween)
            {
                currentState = EnemyState.Idle;
            }
            else
            {
                currentState = EnemyState.Activated;
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && this.gameObject.tag == "Enemy")
        {
            currentState = EnemyState.Idle;
        }

    }

    void MoveToTarget(Vector3 direction)
    {
        //Sets a velocity towards the player
        rb.velocity = new Vector2(direction.x, direction.y) * speed;
    }

    private void OnDestroy()
    {
        UIManager.instance.killedBounder();
    }

    private IEnumerator waitToActivate() {
        yield return new WaitForSeconds(0.5f);
        if (currentState != EnemyState.Idle) {
            currentState = EnemyState.Activated;
        }
    }
}
