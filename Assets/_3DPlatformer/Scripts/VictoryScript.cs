using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScript : MonoBehaviour
{
    public IEnumerator Success()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Victory");
    }

    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Start");
    }

    public void Restart()
    {
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Main");
        }
    }
}
