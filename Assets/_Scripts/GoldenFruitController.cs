using UnityEngine;
using UnityEngine.SceneManagement;

public class GoldenFruitController : MonoBehaviour
{
    public GameObject boss;
    public GameObject fruit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fruit.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (boss == null)
        {
            fruit.SetActive(true);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement player = collision.GetComponent<PlayerMovement>();

        PlayerPrefs.SetInt("Health", player.health);
        PlayerPrefs.SetInt("Flames", player.flames);

        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx + 1);
    }
}
