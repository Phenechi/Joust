using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopScript : MonoBehaviour
{

    public Animator anim;
    public AudioClip poop;
    AudioSource poopSource;
    bool landing;

    // Start is called before the first frame update
    void Start()
    {
        poopSource = AddAudio(false, false, 0.2f);
        poopSource.clip = poop;
        poopSource.Play();
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

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
        {

            anim.SetBool("landing", true);
            StartCoroutine(PoopSplat());


        }
    }

    IEnumerator PoopSplat()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);

    }



}
