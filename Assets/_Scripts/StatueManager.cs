using UnityEngine;
using TMPro;

public class StatueManager : MonoBehaviour
{
    public TextMeshProUGUI msg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        msg.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            msg.gameObject.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        msg.gameObject.SetActive(false);
    }
}
