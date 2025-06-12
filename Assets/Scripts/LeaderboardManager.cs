using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    private const string LEADERBOARD_KEY = "TimeRidersLeaderboard";
    private List<ScoreEntry> scores = new List<ScoreEntry>();
    
    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public float time;
        
        public ScoreEntry(string name, float raceTime)
        {
            playerName = name;
            time = raceTime;
        }
    }
    
    void Start()
    {
        LoadScores();
    }
    
    public void AddScore(string playerName, float time)
    {
        scores.Add(new ScoreEntry(playerName, time));
        scores = scores.OrderBy(s => s.time).ToList(); // Sort by best (lowest) time
        
        // Keep only top 10
        if (scores.Count > 10)
        {
            scores.RemoveRange(10, scores.Count - 10);
        }
        
        SaveScores();
    }
    
    public List<ScoreEntry> GetTopScores(int count = 10)
    {
        return scores.Take(count).ToList();
    }
    
    void SaveScores()
    {
        LeaderboardData data = new LeaderboardData();
        data.scores = scores.ToArray();
        
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }
    
    void LoadScores()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            
            if (data != null && data.scores != null)
            {
                scores = data.scores.ToList();
            }
        }
    }
    
    [System.Serializable]
    public class LeaderboardData
    {
        public ScoreEntry[] scores;
    }
}