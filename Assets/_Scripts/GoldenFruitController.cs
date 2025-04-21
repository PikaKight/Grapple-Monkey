using UnityEngine;
using UnityEngine.SceneManagement;

public class GoldenFruitController : MonoBehaviour
{
    public GameObject boss;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (boss == null)
        {
            gameObject.SetActive (true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "player") return;

        gameObject.SetActive(false);

        SceneManager.LoadScene("End");
    }
}
