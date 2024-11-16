using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager: MonoBehaviour
{
    private string _leaderboardId = "TestLevel";
    public static LeaderboardManager Instance;
    public RectTransform leaderboardPanel;
    public RectTransform leaderboardEntryPrefab;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI timeText;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var entry = await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, 1e6);
        usernameInput.SetTextWithoutNotify(entry.PlayerName.Split('#')[0]);

        usernameInput.characterLimit = 32;
        usernameInput.onEndEdit.AddListener(OnUsernameChanged);
    }
    
    private async void OnUsernameChanged(string username)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(username.Split('#')[0]);
        UpdateLeaderboardData();
    }

    public async void UpdateScore()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, Time.timeSinceLevelLoad);
    }
    
    public void OnShowLeaderboard()
    {
        var playerTime = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        timeText.text = $"{playerTime.Minutes:00}:{playerTime.Seconds:00}:{playerTime.Milliseconds:000}";
        timeText.transform.DOPunchScale(Vector3.one * 0.05f, 4f).SetLoops(-1);
        UpdateLeaderboardData();
        StartCoroutine(UpdateLeaderboard());
    }

    private async void UpdateLeaderboardData()
    {
        var leaderboard = await LeaderboardsService.Instance.GetScoresAsync(_leaderboardId);
        foreach (Transform child in leaderboardPanel)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var entry in leaderboard.Results)
        {
            var entryTransform = Instantiate(leaderboardEntryPrefab, leaderboardPanel);
            var name = (entry.Rank + 1) + ". " + entry.PlayerName.Split('#')[0];
            var nameText = entryTransform.Find("Name").GetComponent<TMP_Text>();
            nameText.text = name;
            if (entry.PlayerId == AuthenticationService.Instance.PlayerId)
            {
                nameText.fontStyle = FontStyles.Bold;
            }
            
            var score = TimeSpan.FromSeconds(entry.Score);
            entryTransform.Find("Time").GetComponent<TextMeshProUGUI>().text = 
                $"{score.Minutes:00}:{score.Seconds:00}:{score.Milliseconds:000}";
        }
    }
    
    private IEnumerator UpdateLeaderboard()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            try
            {
                UpdateLeaderboardData();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}