using UnityEngine;

public class CollectableController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            int flames = PlayerPrefs.GetInt("Flames", 0);

            PlayerPrefs.SetInt("Flames", flames + 1);
        }

        gameObject.SetActive(false);
    }
}
