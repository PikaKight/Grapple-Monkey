using UnityEngine;

public class CollectableController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        switch (gameObject.tag)
        {
            case "Collect":
                int flames = PlayerPrefs.GetInt("Flames", 0);

                PlayerPrefs.SetInt("Flames", flames + 1);

                break;

            case "Health":
                int playerHealth = PlayerPrefs.GetInt("Health", 100);

                PlayerPrefs.SetInt("Health", playerHealth + 1);
                break ;
        }

        gameObject.SetActive(false);
    }
}
