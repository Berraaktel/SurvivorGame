using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpValue = 1;
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
            PlayerXP xp = player.GetComponent<PlayerXP>();
            if (xp != null)
            {
                xp.AddXP(xpValue);
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
