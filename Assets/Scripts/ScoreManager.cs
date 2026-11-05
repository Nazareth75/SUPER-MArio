using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public const int CoinPoints = 200;
    public const int EnemyPoints = 100;
    public const int BossPoints = 1000;

    public int score { get; private set; }

    public event Action ScoreChanged;

    readonly HashSet<int> defeatedEnemies = new HashSet<int>();

    public static void AddCoinPoints()
    {
        Ensure().Add(CoinPoints);
    }

    public static void AddEnemyPoints(UnityEngine.Object enemy)
    {
        if (enemy == null) {
            return;
        }

        ScoreManager manager = Ensure();
        if (!manager.defeatedEnemies.Add(enemy.GetInstanceID())) {
            return;
        }

        manager.Add(EnemyPoints);
    }

    public static void AddBossPoints(UnityEngine.Object boss)
    {
        if (boss == null) {
            return;
        }

        ScoreManager manager = Ensure();
        if (!manager.defeatedEnemies.Add(boss.GetInstanceID())) {
            return;
        }

        manager.Add(BossPoints);
    }

    public static void ResetScore()
    {
        ScoreManager manager = Ensure();
        manager.score = 0;
        manager.defeatedEnemies.Clear();
        manager.ScoreChanged?.Invoke();
    }

    static ScoreManager Ensure()
    {
        if (Instance != null) {
            return Instance;
        }

        ScoreManager existing = FindAnyObjectByType<ScoreManager>();
        if (existing != null) {
            Instance = existing;
            return existing;
        }

        GameObject host = new GameObject("ScoreManager");
        DontDestroyOnLoad(host);
        return host.AddComponent<ScoreManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    void Add(int points)
    {
        score += points;
        ScoreChanged?.Invoke();
    }
}
