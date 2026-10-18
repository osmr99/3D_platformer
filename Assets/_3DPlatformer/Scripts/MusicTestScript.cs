using UnityEngine;

public class MusicTestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.PlayMusic("Test1", 0, 0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            MusicManager.Instance.PlayMusic("Test2", 0.5f, 0.05f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            MusicManager.Instance.PlayMusic("Test1", 0.5f, 0.05f);
    }
}
