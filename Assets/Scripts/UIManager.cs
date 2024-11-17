using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;
    public RectTransform finishedPanel;
    public TextMeshProUGUI clock;
    public RectTransform inGameUI;
    
    public bool gameEnded;
    
    public void Awake()
    {
        Instance = this;
        finishedPanel.gameObject.SetActive(false);
    }
    
    public void ShowFinishedPanel()
    {
        if (gameEnded)
        {
            return;
        }
        inGameUI.DOScale(1.5f, 0.3f).SetEase(Ease.OutElastic).onComplete += () =>
        {
            inGameUI.gameObject.SetActive(false);
        };
        inGameUI.gameObject.SetActive(false);
        finishedPanel.gameObject.SetActive(true);
        finishedPanel.anchorMin = new Vector2(0f, 1f);
        finishedPanel.anchorMax = new Vector2(1f, 2f);
        
        finishedPanel.DOAnchorMax(new Vector2(1f, 1f), 0.3f).SetEase(Ease.InBounce);
        finishedPanel.DOAnchorMin(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InBounce);
        
        LeaderboardManager.Instance.UpdateScore();
        LeaderboardManager.Instance.OnShowLeaderboard();

        gameEnded = true;
    }
    
    public void Update()
    {
        if (gameEnded)
        {
            return;
        }
        var minutes = (int) Time.timeSinceLevelLoad / 60;
        var seconds = (int) Time.timeSinceLevelLoad % 60;
        clock.text = $"{minutes:00}:{seconds:00}";
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void NextLevel()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}