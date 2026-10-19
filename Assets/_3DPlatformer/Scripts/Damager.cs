using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] EnemyHealthScript enemy;
    [SerializeField] PlayerControllerScript playerControllerScript;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!playerControllerScript.isHurt && !enemy.isDead && !enemy.isHurt)
            {
                playerControllerScript.UpdateHP(1);
            }
        }
    }
}
