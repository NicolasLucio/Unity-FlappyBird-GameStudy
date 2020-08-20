using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameSystem : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject barrierPrefab;
    private Rigidbody2D playerBody;
    private int timerInSeconds;
    private int nextBarrier;
    private int timeForNextBarrier;
    private int playerPoints;
    private int highScorePoints;
    private int lastPoints;
    private int difficultyTimer;
    private int pointsMultipliyer;
    private Vector2 playerVector;

    private Text highScoreText;
    private Text scoreText;
    private Text timerText;

    private GameObject[] resetBarrierOnScene;

    public AudioClip audioOne;
    public AudioClip audioTwo;
    public AudioSource primaryAudioSource;
    void Start()
    {
        highScorePoints = 0;
        playerBody = playerObject.GetComponent<Rigidbody2D>();
        highScoreText = GameObject.Find("HighScore").GetComponent<Text>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        timerText = GameObject.Find("Timer").GetComponent<Text>();

        IniciateMatch();
    }

    void IniciateMatch()
    {
        timerText.text = "Timer: 0";

        playerBody.gravityScale = 0.0f;
        playerBody.velocity = Vector2.zero;
        playerBody.angularVelocity = 0.0f;
        playerBody.angularDrag = 0.0f;
        transform.rotation = Quaternion.identity;

        timerInSeconds = 0;
        nextBarrier = 0;
        timeForNextBarrier = 3;
        playerPoints = 0;
        difficultyTimer = 0;
        pointsMultipliyer = 1;
        StartCoroutine(GameTimer());

        this.transform.position = new Vector2(-6.5f, 0.0f);
    }
    
    void Update()
    {
        playerVector = this.transform.position;
        if (timerInSeconds >= 2)
        {   
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerBody.AddForce(new Vector2(0.0f, 10.0f), ForceMode2D.Impulse);
                PlayAudio(0);
            }
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        playerBody.AddForce(new Vector2(0.0f, 10.0f), ForceMode2D.Impulse);
                        PlayAudio(0);
                        break;
                }
            }            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (playerVector.y > 6.5f || playerVector.y < -5.5f)
        {   
            CheckScore(2);
        }
    }

    void CreateBarrier()
    {
        float randomY = Random.Range(-3.40f, 3.41f);
        GameObject tempObject = Instantiate(barrierPrefab, new Vector2(10.0f, randomY), Quaternion.identity);        
    }

    void CheckScore (int whichScore)
    {
        if (whichScore == 1)
        {
            scoreText.text = "Score: " + playerPoints.ToString();
        }
        else if (whichScore == 2)
        {
            PlayAudio(1);
            //stop timer
            StopAllCoroutines();

            //change last score
            scoreText.text = "Score : 0";

            lastPoints = playerPoints;
            if (lastPoints > highScorePoints)
            {
                highScoreText.text = "High Score: " + lastPoints;
                highScorePoints = lastPoints;
            }
            
            //delete barrier
            resetBarrierOnScene = GameObject.FindGameObjectsWithTag("Finish");
            foreach (GameObject aux in resetBarrierOnScene)
            {
                aux.SetActive(false);
                Destroy(aux);
            }
            //new game
            IniciateMatch();
        }
    }

    void PlayAudio(int which)
    {
        if (which == 0)
        {
            primaryAudioSource.clip = audioOne;
            primaryAudioSource.Play();
        }
        else if (which == 1)
        {
            primaryAudioSource.clip = audioTwo;
            primaryAudioSource.Play();
        }        
    }

    IEnumerator GameTimer()
    {
        while (true)
        {   
            yield return new WaitForSeconds(1.0f);
            timerInSeconds++;
            timerText.text = "Timer: " + timerInSeconds.ToString();
            nextBarrier++;
            difficultyTimer++;
            if (nextBarrier == timeForNextBarrier)
            {
                CreateBarrier();
                nextBarrier = 0;
            }
            if (timerInSeconds == 2)
            {
                playerBody.gravityScale = 1.0f;
            }

            if (difficultyTimer == 20)
            {
                if (timeForNextBarrier > 1)
                {
                    timeForNextBarrier--;
                    nextBarrier = 0;
                }
                pointsMultipliyer++;
                difficultyTimer = 0;
            }            
        }        
    }

    void OnCollisionEnter2D (Collision2D collision)
    {   
        CheckScore(2);
    }

    void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.tag == "Point")
        {
            if (pointsMultipliyer == 1)
            {
                playerPoints++;
            }
            else
            {
                playerPoints = playerPoints * pointsMultipliyer;
            }            
            CheckScore(1);
        }
    }
}
