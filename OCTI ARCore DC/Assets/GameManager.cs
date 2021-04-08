using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public GameObject faceManager;
    public GameObject goalCollider;
    private GameObject ballPrefab;
    private UIManager ui;
    private AudioSource audioSource;
    public AudioClip shortBuzzer, longBuzzer;

    float spawnTime = 2f;
    float elapsed = 0;
    public int score = 0;
    bool gameStart = false;

    float buzzerTime = 24;
    public float buzzerElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        ballPrefab = Resources.Load<GameObject>("Basketball");
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shortBuzzer;
        ui = FindObjectOfType<UIManager>();
        faceManager.SetActive(true);
        buzzerTime++;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStart || ui.pause)
            return;
        
        //Update UI
        ui.updateUI(((int)buzzerElapsed), score.ToString());
        buzzerElapsed -= Time.deltaTime;
        
        if (buzzerElapsed <= 0)
        {
            endGame();
        }
        
        //Spawn Balls
        elapsed += Time.deltaTime;
        while (elapsed > spawnTime)
        {
            GameObject temp = Instantiate(ballPrefab, new Vector3(Random.Range(-.065f, .065f), .25f, Random.Range(0.43f, 0.35f)), Quaternion.identity);
            Destroy(temp, 15f);
            elapsed = 0;
        }
    }
    public void startGame()
    {
        gameStart = true;
        goalCollider.SetActive(true);
        buzzerElapsed = buzzerTime;
        score = 0;

        if(audioSource.clip != shortBuzzer)
            audioSource.clip = shortBuzzer;
        audioSource.Play();

        Debug.Log("Game Start");
    }

    public void resetTimer()
    {
        if (buzzerElapsed > 0)
        {
            buzzerElapsed += 5f;
            if (buzzerElapsed > 24)
                buzzerElapsed = 24;
        }
    }

    public void endGame()
    {
        if(audioSource.clip != longBuzzer)
            audioSource.clip = longBuzzer;
        audioSource.Play();
        GameObject[] allBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in allBalls)
            Destroy(ball);

        goalCollider.SetActive(false);
        gameStart = false;
        ui.openPostGameUI();
    }
}
