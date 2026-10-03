using UnityEngine;

// Central "the run gets harder over time" scaling, layered on top of
// EnemySpawner's own spawn-rate ramp and EnemyPool's level-gated enemy
// variety: those two control how OFTEN and WHICH enemies show up, this
// controls how TOUGH each individual enemy is. Without this a Scout
// spawned at minute 1 and a Scout spawned at minute 15 hit exactly as
// hard - which made the permanent Shop upgrades matter less the longer a
// run went, instead of more.
public static class DifficultyManager
{
    // 1x at the start, 2x at the 5-minute mark, 3x at 10 minutes and
    // beyond - a steady climb that still caps out instead of scaling
    // forever into unfair numbers late in a long run.
    private const float ScalePerSecond = 1f / 300f;
    private const float MaxMultiplier = 3f;

    private static SurvivalTimerUI cachedTimer;

    public static float GetMultiplier()
    {
        float elapsed = GetElapsedSeconds();
        return Mathf.Clamp(1f + elapsed * ScalePerSecond, 1f, MaxMultiplier);
    }

    public static float GetElapsedSeconds()
    {
        // SurvivalTimerUI lives on the run's Canvas and is destroyed and
        // recreated on every scene reload, so the cached reference must be
        // re-validated (Unity's Object == null catches a destroyed one)
        // rather than fetched once for the static class's lifetime.
        if (cachedTimer == null)
        {
            cachedTimer = Object.FindFirstObjectByType<SurvivalTimerUI>();
        }
        return cachedTimer != null ? cachedTimer.Elapsed : 0f;
    }
}
