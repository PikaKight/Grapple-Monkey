using UnityEngine;

public class CollectableController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        int flames = PlayerPrefs.GetInt("Flames", 0);

        if (tag == "Collect")
            flames++;
        else if (tag == "Health")
        {
            int hp = PlayerPrefs.GetInt("Health", 100);
            PlayerPrefs.SetInt("Health", hp + 1);
        }

        PlayerPrefs.SetInt("Flames", flames);
        gameObject.SetActive(false);

        // (optional) if you have an on‑screen coin UI,
        // find its script and call an update method here.
    }
}
