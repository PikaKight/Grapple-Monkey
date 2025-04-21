using UnityEngine;
using TMPro;


public class CollectableController : MonoBehaviour
{
    public int health = 2;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();

        switch (gameObject.tag)
        {
            case "Collect":

                int flames = PlayerPrefs.GetInt("Flames", 0);
                flames++;
                PlayerPrefs.SetInt("Flames", flames);

                player.changeFlames(flames);

                break;

            case "Health":

                player.changeHealth(health);

                break;
        }

        gameObject.SetActive(false);
    }
}
