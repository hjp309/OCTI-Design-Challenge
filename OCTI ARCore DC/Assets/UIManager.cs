using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public Image background;
    public GameObject blurObj;
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI buzzerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreHeader;
    public TextMeshProUGUI restartButton;
    public Button menuButton;
    public Scrollbar menuScrollBar;
    public SwipeContent swiper;
    public Material hoopMaterial;
    private Material blurMaterial;
    public float blurSize;
    public float defaultBlurSize;
    public float fadeDuration;
    private GameManager gameManager;
    private bool firstTime = true;

    public GameObject[] instructionPanels;
    public bool pause = false;

    // Start is called before the first frame update
    void Start()
    {
        buzzerText.color = new Color(1, 1, 1, 0);

        blurMaterial = blurObj.GetComponent<Image>().material;
        blurMaterial.SetFloat("_Size", defaultBlurSize);
        blurSize = defaultBlurSize;
        gameManager = FindObjectOfType<GameManager>();
        hoopMaterial.DOFade(0, 0);

        //Not your first time
        if (PlayerPrefs.GetInt("FirstTime") != 0)
        {
            foreach (GameObject panel in instructionPanels)
                panel.SetActive(false);

            Debug.Log("Not player's first time - Hide Instructions");
        }
        else
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player's first time - Show Instructions.");
        }
    }

    public void closeWelcomeUI()
    {
        blurObj.GetComponent<Button>().interactable = false;
        welcomeText.transform.DOLocalMoveY(200f, fadeDuration);

        StartCoroutine(blur(fadeDuration, true));

        background.DOFade(0, fadeDuration);
        welcomeText.DOFade(0, fadeDuration).OnComplete(() => openGameUI());
    }

    public void openMenu()
    {
        pause = true;
        background.DOFade(1, fadeDuration);
        welcomeText.gameObject.SetActive(true);
        welcomeText.transform.DOLocalMoveY(0, 0);
        blurObj.GetComponent<Button>().interactable = true;

        StartCoroutine(blur(fadeDuration));

        welcomeText.DOFade(1, 1);
        foreach (GameObject panel in instructionPanels)
            panel.SetActive(true);
    }

    public void openPostGameUI()
    {
        buzzerText.gameObject.SetActive(false);
        scoreHeader.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        background.DOFade(defaultBlurSize, fadeDuration);
        welcomeText.gameObject.SetActive(true);
        welcomeText.color = Color.black;
        welcomeText.text = "Final Score: " + scoreText.text;
        welcomeText.transform.DOLocalMoveY(0, fadeDuration);
        welcomeText.DOFade(1, fadeDuration);
        hoopMaterial.DOFade(0, fadeDuration);

        StartCoroutine(spawnRestartButton(3f));
    }

    public void updateUI(int buzzerTime, string score)
    {
        buzzerText.text = buzzerTime.ToString();

        if (buzzerTime <= 10)
        {
            bool red = (int)buzzerTime % 2 == 0;
            buzzerText.color = red ? Color.red : Color.white;
        }

        scoreText.text = score;
    }

    private IEnumerator spawnRestartButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        restartButton.gameObject.SetActive(true);
        restartButton.DOFade(1f, 0.5f);
    }

    public void restart()
    {
        welcomeText.text = "Please make sure your forehead is visible before starting. \n\n Once you're ready, tap anywhere to continue!";
        restartButton.gameObject.SetActive(false);
        background.DOFade(0f, fadeDuration).OnComplete(() => blurObj.SetActive(false));
        welcomeText.color = Color.white;
        openGameUI();
        Debug.Log("Game restarted");
    }

    private IEnumerator blur(float time, bool unblur = false)
    {
        if (unblur)
        {
            blurObj.SetActive(true);

            DOTween.To(() => blurSize, x => blurSize = x, defaultBlurSize, fadeDuration);
            float elapsed = 0;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                blurMaterial.SetFloat("_Size", blurSize);
                yield return null;
            }

            blurObj.SetActive(false);
        }
        else
        {
            blurObj.SetActive(true);
            DOTween.To(() => blurSize, x => blurSize = x, 0f, fadeDuration);
            float elapsed = 0;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                blurMaterial.SetFloat("_Size", blurSize);
                yield return null;
            }
        }
    }

    private void openGameUI()
    {
        welcomeText.gameObject.SetActive(false);
        buzzerText.DOFade(1, 0.5f);
        hoopMaterial.DOFade(1, 0.5f).OnComplete(()=> StartCoroutine(StartCountdown()));
    }

    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        float t = 3;
        while (t > 0)
        {
            t -= Time.deltaTime;
            countdownText.text = ((int)t + 1).ToString();

            yield return null;
        }

        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();
        }

        foreach (GameObject panel in instructionPanels)
            panel.SetActive(false);

        menuScrollBar.value = 0;
        swiper.scroll_pos = 0;
        pause = false;
        countdownText.gameObject.SetActive(false);
        buzzerText.gameObject.SetActive(true);
        scoreHeader.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        gameManager.startGame();
    }
}
