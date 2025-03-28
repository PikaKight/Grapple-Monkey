using UnityEngine;

public class RuinManager : MonoBehaviour
{
    public GameObject spike;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            spike.SetActive(false);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
}

