using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager: MonoBehaviour
{
    private string _leaderboardId = "TestLevel";
    public static LeaderboardManager Instance;
    
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

        await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, 1e6);
    }

    public async void UpdateScore()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, Time.timeSinceLevelLoad);
    }
    
    public async void OnShowLeaderboard()
    {
        var leaderboard = await LeaderboardsService.Instance.GetScoresAsync(_leaderboardId);
        foreach (var entry in leaderboard.Results)
        {
            Debug.Log($"{entry.Rank}. {entry.PlayerId} - {entry.Score}");
        }
    }
}