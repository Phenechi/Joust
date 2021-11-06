using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToLose()
    {
        SceneManager.LoadScene("OverScene");
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length != 0) { 
            for (int i = 0; i < player.Length; i ++) {
                Destroy(player[i].gameObject);
            }
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length != 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i].gameObject);
            }
        }
    }

    public void MainButton()
    {
        SceneManager.LoadScene("StartScene");
    }

    
}
