using Unity.VisualScripting;
using UnityEngine;

public class GoalTriggerScript : MonoBehaviour
{
    PlayerControllerScript playerControllerScript;
    VictoryScript victoryScript;
    [SerializeField] GameObject goalText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerControllerScript = FindFirstObjectByType<PlayerControllerScript>();
        victoryScript = FindFirstObjectByType<VictoryScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MusicManager.Instance.StopMusic();
            goalText.SetActive(false);
            playerControllerScript.DisableInputs();
            playerControllerScript.sounds.PlayOneShot(playerControllerScript.victorySfx);
            victoryScript.StartCoroutine("Success");
        }
    }
}
