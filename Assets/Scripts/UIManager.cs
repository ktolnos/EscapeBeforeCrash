using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;
    public RectTransform finishedPanel;
    public TextMeshProUGUI clock;
    
    private bool _gameEnded;
    
    public void Awake()
    {
        Instance = this;
        finishedPanel.gameObject.SetActive(false);
    }
    
    public void ShowFinishedPanel()
    {
        if (_gameEnded)
        {
            return;
        }
        finishedPanel.gameObject.SetActive(true);
        finishedPanel.anchorMin = new Vector2(0f, 1f);
        finishedPanel.anchorMax = new Vector2(1f, 2f);
        
        finishedPanel.DOAnchorMax(new Vector2(1f, 1f), 0.3f).SetEase(Ease.InBounce);
        finishedPanel.DOAnchorMin(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InBounce);
        
        LeaderboardManager.Instance.UpdateScore();
        LeaderboardManager.Instance.OnShowLeaderboard();

        _gameEnded = true;
    }
    
    public void Update()
    {
        if (_gameEnded)
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
}