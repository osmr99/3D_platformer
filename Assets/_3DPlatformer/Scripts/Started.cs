using UnityEngine;
using UnityEngine.SceneManagement;

public class Started : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Main");
    }
}
