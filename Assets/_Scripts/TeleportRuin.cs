using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportRuin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            int activeScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeScene + 1);
        }
    }
}
