using UnityEngine;
using TMPro;


public class CollectableController : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        switch (gameObject.tag)
        {
            case "Collect":
               
                int flames = PlayerPrefs.GetInt("Flames", 0);
                flames++;
                PlayerPrefs.SetInt("Flames", flames);

                statusText.text = $"Sacred Flames: {flames}";

                break;

            case "Health":
                int hp = PlayerPrefs.GetInt("Health", 100);
                hp++;
                PlayerPrefs.SetInt("Health", hp);
                //statusText.text = $"Health: {hp}";
                break;
        }

        gameObject.SetActive(false);
    }
}
