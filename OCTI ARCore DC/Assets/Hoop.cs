using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    public ParticleSystem confetti;
    private GameManager gameManager;
    private GameObject swishSFX;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        swishSFX = Resources.Load<GameObject>("SwishSound");
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            Destroy(Instantiate(swishSFX), 5f);
            gameManager.score++;
            gameManager.resetTimer();
            confetti.Play();
            Destroy(other.gameObject, 2f);
        }
    }
}
