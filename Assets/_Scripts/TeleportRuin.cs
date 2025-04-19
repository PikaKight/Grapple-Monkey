using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportRuin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx + 1);
    }
}
