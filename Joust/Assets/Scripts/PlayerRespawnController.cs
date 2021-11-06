using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    public static PlayerRespawnController instance;

    public GameObject player;
    GameObject[] spawnPlatforms;
    public AudioClip playerDie;
    AudioSource playerDieSource;
    public float timeToRespawn;
    bool respawning;

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

    public AudioSource AddAudio(bool loop, bool playOnAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playOnAwake;
        newAudio.volume = vol;
        return newAudio;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerDieSource = AddAudio(false, false, 0.5f);
        respawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        spawnPlatforms = GameObject.FindGameObjectsWithTag("ActiveSpawner");
        if (spawnPlatforms.Length == 0)
        {
            spawnPlatforms = GameObject.FindGameObjectsWithTag("NotActiveSpawner");
        }
        if ((GameObject.FindGameObjectsWithTag("Player")).Length == 0 && respawning == false)
        {
            respawning = true;
            playerDieSource.clip = playerDie;
            playerDieSource.Play();
        }
    }

    public void Respawn() {
        if ((GameObject.FindGameObjectsWithTag("Player")).Length == 0 && UIManager.instance.GetLives() > 0)
        {
            int index = Random.Range(0, spawnPlatforms.Length);
            Instantiate(player, spawnPlatforms[index].transform.position, spawnPlatforms[index].transform.rotation);
            respawning = false;
        }

    }
}
