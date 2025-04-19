using UnityEngine;

public class RuinManager : MonoBehaviour
{
    public GameObject spike;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            spike.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
}
