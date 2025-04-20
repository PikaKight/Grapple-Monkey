using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportRuin : MonoBehaviour
{
    public GameObject boss;
    public GameObject rune;

    void Start()
    {
        rune.SetActive(false);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    void Update()
    {
        if (boss == null)
        {
            rune.SetActive(true);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx + 1);
    }
}
