using UnityEngine;
using System.Collections.Generic;

// Fifth weapon archetype: an instant hitscan strike on the nearest enemy
// in range that then chains to up to maxChainTargets-1 additional nearby
// enemies. Unlike every other weapon here the hit is immediate (no
// projectile travel time, no self-centered pulse) - it rewards standing
// near a cluster so the chain has somewhere to jump to, same spirit as
// DustNova but single-file instead of a ring. Starts disabled - unlocked
// as a level-up choice (see PlayerUpgrades.UnlockLightning/UpgradeUI).
public class LightningWeapon : MonoBehaviour
{
    public float strikeInterval = 1.8f;
    public float range = 4f;
    public int damage = 2;
    public int maxChainTargets = 3;
    public float chainRadius = 2.5f;
    public Color boltColor = new Color(0.65f, 0.85f, 1f);

    // Evolution: max level + Miknatis (Magnet) turns this into "Simsek
    // Firtinasi" - chains much further and to far more targets. Same
    // shape as every other weapon here, see PlayerAttack for the full
    // explanation.
    public int level = 1;
    public int maxLevel = 5;
    public bool evolved = false;

    private float timer;
    private HashSet<EnemyHealth> hitBuffer = new HashSet<EnemyHealth>();

    void OnEnable()
    {
        // Same re-zero-on-enable reasoning as RockBombWeapon.OnEnable -
        // otherwise the first strike fires instantly off whatever time
        // accumulated while this sat disabled since Awake.
        timer = 0f;
    }

    public bool CanLevelUp()
    {
        return enabled && !evolved && level < maxLevel;
    }

    public void LevelUp()
    {
        level++;
        damage += 1;
        strikeInterval = Mathf.Max(0.9f, strikeInterval - 0.15f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= strikeInterval)
        {
            timer = 0f;
            TryStrike();
        }
        CheckEvolution();
    }

    void CheckEvolution()
    {
        if (evolved || level < maxLevel) return;

        PlayerUpgrades pu = GetComponent<PlayerUpgrades>();
        if (pu == null || !pu.HasMagnet()) return;

        evolved = true;
        damage += 1;
        maxChainTargets += 3;
        chainRadius += 1.5f;
        range += 1f;
        boltColor = new Color(1f, 0.9f, 0.5f);

        if (EvolutionBanner.Instance != null) EvolutionBanner.Instance.Show(Localization.Get("evo_lightning"));
        FxSpawner.Burst(transform.position, new Color(1f, 0.85f, 0.35f), 32, 3.2f, 0.65f, 0.22f);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.28f, 0.2f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();
    }

    void TryStrike()
    {
        EnemyHealth first = FindNearest(transform.position, range, null);
        if (first == null) return;

        hitBuffer.Clear();
        Vector3 previousPoint = transform.position;
        EnemyHealth current = first;

        for (int i = 0; i < maxChainTargets && current != null; i++)
        {
            current.TakeDamage(damage);
            hitBuffer.Add(current);

            DrawBoltSegment(previousPoint, current.transform.position);
            FxSpawner.Burst(current.transform.position, boltColor, 10, 2.2f, 0.2f, 0.11f);

            previousPoint = current.transform.position;
            current = FindNearest(previousPoint, chainRadius, hitBuffer);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.05f, 0.04f);
    }

    // No LineRenderer/material needed - a handful of tiny bursts sampled
    // along the segment reads as a bolt connecting the two points, using
    // the exact same FxSpawner tech every other weapon's feedback already
    // relies on, so there's no new shader/material risk under URP.
    void DrawBoltSegment(Vector3 from, Vector3 to)
    {
        int samples = 4;
        for (int s = 1; s <= samples; s++)
        {
            float t = (float)s / (samples + 1);
            Vector3 point = Vector3.Lerp(from, to, t);
            FxSpawner.Burst(point, boltColor, 3, 1.2f, 0.12f, 0.07f);
        }
    }

    EnemyHealth FindNearest(Vector3 from, float searchRadius, HashSet<EnemyHealth> exclude)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, searchRadius);
        EnemyHealth nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
            if (enemy == null) continue;
            if (exclude != null && exclude.Contains(enemy)) continue;
            // Same on-screen rule as the other weapons - a chain hop can
            // easily reach past the edge of the screen even when the
            // strike that started it was aimed at something visible.
            if (CameraFollow.Instance != null && !CameraFollow.Instance.IsOnScreen(hits[i].transform.position)) continue;

            float dist = (hits[i].transform.position - from).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
