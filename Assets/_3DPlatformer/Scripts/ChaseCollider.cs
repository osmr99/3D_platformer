using UnityEngine;

public class ChaseCollider : MonoBehaviour
{
    [SerializeField] EnemyHealthScript enemy;

    private void Update()
    {
        if (enemy == null)
            Destroy(gameObject);
        else
        {
            transform.position = enemy.transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !enemy.isHurt && !enemy.isDead)
        {
            enemy.state = EnemyStates.CHASE;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !enemy.isHurt && !enemy.isDead)
        {
            enemy.EndChasing();
        }
    }
}
