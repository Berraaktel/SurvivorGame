using UnityEngine;

// Flying Gold pickup, mirrors XPOrb exactly (same magnet/settle/pickup
// behaviour) but feeds PlayerGold's run-local counter instead of XP.
public class GoldOrb : MonoBehaviour
{
    public int goldValue = 1;
    public float magnetRange = 2.5f;
    public float pickupRange = 0.3f;
    public float moveSpeed = 8f;
    public float spawnSettleTime = 0.5f;

    private Transform player;
    private float spawnTime;

    void OnEnable()
    {
        spawnTime = Time.time;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        if (Time.time - spawnTime < spawnSettleTime) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= pickupRange)
        {
            PlayerGold gold = player.GetComponent<PlayerGold>();
            if (gold != null)
            {
                gold.AddGold(goldValue);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayPickup();
            gameObject.SetActive(false);
            return;
        }

        if (dist <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }
}
