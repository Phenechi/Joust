using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    public static EnemySpawnerController instance;

    public GameObject bounder;
    public GameObject hunter;
    public GameObject shadowLord;
    public GameObject roosters;
    public GameObject seagulls;
    public GameObject cow;

    //two spawnpoint for roosters and seagulls
    public GameObject spawnpoint;
    public GameObject spawnpoint2;

    GameObject enemyToSpawn;

    public int enemyLimit;

    GameObject[] enemies;
    GameObject[] notStartedEnemies;
    GameObject[] eggs;
    GameObject[] cows;
    GameObject[] spawnPlatforms;
    int numEnemies;

    public float spawnInterval;

    float timer = 0.0f;
    int wave;

    bool startingNewWave;
    float bounderChance;
    float hunterChance;
    float shadowLordChance;

    float bounderRange;
    float hunterRange;
    float shadowLordRange;

    //for spawning the seagulls and the roosters
    int totalFlying;
    public float spawnRate;
    float spawnTimer;
    int currentFlying;
    int cowsSpawned;
    int cowLimit;

    //for internal spawning of the seagulls
    float seagullTimer;
    public float seagullInterval;

    void Start() {
        numEnemies = 0;
        startingNewWave = false;
        bounderChance = 1;
        hunterChance = 0;
        shadowLordChance = 0;
        bounderRange = 1;
        hunterRange = 0;
        shadowLordRange = 1;
        wave = 1;
        totalFlying = 0;
        cowsSpawned = 0;
        cowLimit = 0;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        spawnPlatforms = GameObject.FindGameObjectsWithTag("ActiveSpawner");
        if (spawnPlatforms.Length == 0)
        {
            spawnPlatforms = GameObject.FindGameObjectsWithTag("NotActiveSpawner");
        }

        if (timer > spawnInterval) {

            if (numEnemies < enemyLimit ) {
                float randomNum = Random.Range(0.0f, 1.0f);
                //print("Random Num: " + randomNum);
                 
                if (randomNum < bounderRange) {
                    enemyToSpawn = bounder;
                }
                else if (randomNum > bounderRange && randomNum < hunterRange) {
                    enemyToSpawn = hunter;
                }
                else if (randomNum > hunterRange && randomNum < shadowLordRange) {
                    enemyToSpawn = shadowLord;
                }
                int index = Random.Range(0, spawnPlatforms.Length);
                Instantiate(enemyToSpawn, spawnPlatforms[index].transform.position, spawnPlatforms[index].transform.rotation);
                timer = 0.0f;
                numEnemies ++;
            }
            
        }
        
        if(spawnTimer > spawnInterval + 0.2 * wave)
        {
            if(currentFlying < totalFlying && seagullTimer > seagullInterval)
            {
                float random = Random.Range(0.0f, 1.0f);
                if(random >= 0.5)
                {
                    Instantiate(seagulls, spawnpoint.transform.position, transform.rotation);
                }
                if(random < 0.5)
                {
                    if(wave>=10)
                        Instantiate(roosters, spawnpoint2.transform.position, transform.rotation);
                    else
                        Instantiate(seagulls, spawnpoint2.transform.position, transform.rotation);
                }


                currentFlying++;
                seagullTimer = 0;

            }
            else
            {
                seagullTimer += Time.deltaTime;
            }

        }

        cows = GameObject.FindGameObjectsWithTag("Cow");
        if (cowsSpawned < cowLimit && cows.Length == 0) {
            cowsSpawned++;
            Instantiate(cow, new Vector3(0, -4.6f, 0), transform.rotation);
        }

        spawnTimer += Time.deltaTime;
        timer += Time.deltaTime;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        notStartedEnemies = GameObject.FindGameObjectsWithTag("NotStartedEnemy");
        eggs = GameObject.FindGameObjectsWithTag("Egg");
        if (enemies.Length == 0 && notStartedEnemies.Length == 0 && eggs.Length == 0 && numEnemies == enemyLimit && startingNewWave == false)
        {
            startingNewWave = true;
            StartCoroutine(delayNextWave());
        }

    }

    public int GetWave()
    {
        return wave;
    }

    public void newWave() {
        numEnemies = 0;
        if (wave % 3 == 0) {
            enemyLimit++;
        }
        if (wave % 3 == 0 && wave >= 6) {
            totalFlying++;
        }
        if (wave >= 6) {
            cowLimit = 1;
        }
        currentFlying = 0;
        cowsSpawned = 0;
        spawnTimer = 0;
        startingNewWave = false;
    }

    private IEnumerator delayNextWave()
    {
        wave++;
        if (shadowLordChance != 1) {
            if (wave > 5 && wave < 10)
            {
                hunterChance += 0.05f;
                bounderChance -= 0.05f;
            }
            else if (wave > 10)
            {
                hunterChance += 0.05f;
                shadowLordChance += 0.05f;
                bounderChance -= 0.1f;
            }
        }

        if (bounderChance < 0) {
            float amountLess = 0 - bounderChance;
            bounderChance = 0;
            hunterChance -= amountLess;
        }
        if (hunterChance < 0) {
            float amountLess = 0 - hunterChance;
            hunterChance = 0;
            shadowLordChance -= amountLess;
        }
        if (shadowLordChance > 1)
        {
            shadowLordChance = 1;
        }
        bounderRange = bounderChance;
        hunterRange = bounderChance + hunterChance;
        if (wave != 6) {
            UIManager.instance.incrementWave();
            yield return new WaitForSeconds(3);
        }
        else {
            yield return new WaitForSeconds(1);
            UIManager.instance.incrementWave();
            yield return new WaitForSeconds(4);
        }
        newWave();
    }
}
