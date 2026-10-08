using System.Transactions;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    Rigidbody rb;
    public float upwardForce = 5;

    public bool isCollected = false;
    public bool canCollect = false;

    private void Awake()
    {
        Invoke("CollectCoin", 0.75f);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isCollected) transform.Rotate(0, 1080 * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && canCollect)
        {
            isCollected = true;
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
            Destroy(gameObject, 0.5f);
        }
    }

    void CollectCoin()
    {
        canCollect = true;
    }    
}
