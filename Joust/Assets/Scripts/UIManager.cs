using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Text lives;
    public Text score;
    public Text waves;
    public Image newWave;
    public Image level2Counter;
    public Sprite level2In3;
    public Sprite level2In2;
    public Sprite level2In1;

    public AudioClip newWaveJingle;
    public AudioClip deathJingle;
    AudioSource newWaveJingleSource;
    AudioSource deathJingleSource;

    public int numLives;
    int totalLives;

    int playerScore = 0;

    int numEggs = 0;

    int waveCounter;
    float deadTimer;

    float dyingTimer;
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
        waveCounter = 1;
        totalLives = numLives;
        lives.text = "Lives: " + numLives.ToString();
        score.text = "Score: " + playerScore.ToString();
        waves.text = "Wave: " + waveCounter.ToString();
        newWave.gameObject.SetActive(false);
        StartCoroutine(hideText());
        level2Counter.enabled = false;
        newWaveJingleSource = AddAudio(false, false, 0.2f);
        deathJingleSource = AddAudio(false, false, 0.5f);
        deadTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        deadTimer += Time.deltaTime;
    }

    public int GetLives() {
        return numLives;
    }

    public void LostLife()
    {
        if (deadTimer > 1) {
            //Subtracts a life every time the player dies
            numLives -= 1;
            lives.text = "Lives: " + numLives.ToString();
            playerScore += 50;
            score.text = "Score: " + playerScore.ToString();
            if (numLives == 0)
            {
                StartCoroutine(waitToEnd());
            }
            deadTimer = 0;
        }
    }

    public void killedBounder() {
        playerScore += 500;
        score.text = "Score: " + playerScore.ToString();
    }

    public void killedHunter()
    {
        playerScore += 750;
        score.text = "Score: " + playerScore.ToString();
    }

    public void killedShadowLord()
    {
        playerScore += 1000;
        score.text = "Score: " + playerScore.ToString();
    }

    public void killedSeagullOrRooster()
    {
        playerScore += 1000;
        score.text = "Score: " + playerScore.ToString();
    }

    public void GotEgg() {
        numEggs++;
        if (numEggs < 4) {
            playerScore += 250 * numEggs;
        }
        else {
            playerScore += 1000;
        }
        score.text = "Score: " + playerScore.ToString();
    }


    public void incrementWave() {
        newWaveJingleSource.clip = newWaveJingle;
        newWaveJingleSource.Play();
        if (waveCounter != 5) {
            waveCounter++;
            waves.text = "Wave: " + waveCounter;
            newWave.gameObject.SetActive(true);
            StartCoroutine(hideText());
        }
        else {
            waveCounter++;
            waves.text = "Wave: " + waveCounter;
            level2Counter.enabled = true;
            StartCoroutine(level2CounterCountdown());
        }

    }

    private IEnumerator hideText() {
        yield return new WaitForSeconds(2);
        newWave.gameObject.SetActive(false);
    }

    private IEnumerator level2CounterCountdown() {
        level2Counter.GetComponent<Image>().sprite = level2In3;
        yield return new WaitForSeconds(1);
        level2Counter.GetComponent<Image>().sprite = level2In2;
        yield return new WaitForSeconds(1);
        level2Counter.GetComponent<Image>().sprite = level2In1;
        yield return new WaitForSeconds(1);
        level2Counter.enabled = false;
    }

    IEnumerator waitToEnd() {
        GameMusicController.instance.StopMusic();
        deathJingleSource.clip = deathJingle;
        deathJingleSource.Play();
        yield return new WaitForSeconds(2.8f);
        GameManager.instance.GoToLose();
    }
}
