using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    
    public void ToMenu()
    {
        SceneManager.LoadScene("Level 1");
    }
}
