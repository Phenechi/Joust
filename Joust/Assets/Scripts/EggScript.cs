using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    float time = 0;
    public float timeToHatch;
    public GameObject newEnemy;

    public Animator anim;
    public AudioClip eggCrack;
    AudioSource eggCrackSource;
    bool waiting;

    enum EggState
    {
        Idle,
        Hatching
    }

    EggState egg;
    // Start is called before the first frame update
    void Start()
    {
        egg = EggState.Idle;
        eggCrackSource = AddAudio(false, false, 0.5f);
        waiting = false;
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
        if (time > timeToHatch && egg == EggState.Idle && waiting == false) {
            waiting = true;
            StartCoroutine(soonToCrack());

        }
        if (!(transform.position.x < 10.5f && transform.position.x > -10.5f) || !(transform.position.y < 5.3f && transform.position.y > -3.8f))
        {
            Destroy(this.gameObject);
        }
        time += Time.deltaTime;
    }
    IEnumerator soonToCrack() {
        eggCrackSource.clip = eggCrack;
        eggCrackSource.Play();
        yield return new WaitForSeconds(1.25f);
        egg = EggState.Hatching;
        anim.SetBool("broke", false);
        StartCoroutine(CallToHatch());
    }
    IEnumerator CallToHatch()
    {
        yield return new WaitForSeconds(1);
        Instantiate(newEnemy, new Vector2(transform.position.x, transform.position.y + 1), transform.rotation);
        Destroy(this.gameObject);


    }




}
