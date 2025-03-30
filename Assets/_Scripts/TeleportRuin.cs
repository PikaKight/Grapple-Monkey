using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportRuin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level 2");
        }
    }
}
