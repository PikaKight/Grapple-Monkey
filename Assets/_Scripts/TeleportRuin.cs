using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportRuin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 2");
    }
}
