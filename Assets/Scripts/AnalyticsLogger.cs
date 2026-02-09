using Firebase.Analytics;
using UnityEngine;

public static class AnalyticsLogger
{
    private static bool Ready => FirebaseInitializer.IsFirebaseReady;

    public static void LogGameStart()
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent("game_start");
    }

    public static void LogGameOver(int score, int bestScore)
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent(
            "game_over",
            new Parameter("score", score),
            new Parameter("best_score", bestScore)
        );
    }

    public static void LogScoreMilestone(int score)
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent(
            "score_milestone",
            new Parameter("score", score)
        );
    }

    public static void LogComboActivated()
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent("combo_double_activated");
    }

    public static void LogRodSpawned(int score)
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent(
            "rod_spawned",
            new Parameter("score", score)
        );
    }

    public static void LogPerfectJump()
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent("perfect_jump");
    }

    public static void LogSaveMeUsed(int score)
    {
        if (!Ready) return;

        FirebaseAnalytics.LogEvent(
            "save_me_used",
            new Parameter("score", score)
        );
    }
}
