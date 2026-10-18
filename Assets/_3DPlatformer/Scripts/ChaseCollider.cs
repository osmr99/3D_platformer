using UnityEngine;

public class ChaseCollider : MonoBehaviour
{
    EnemyHealthScript enemy;

    private void Awake()
    {
        enemy = FindFirstObjectByType<EnemyHealthScript>();
    }

    private void Update()
    {
        if (enemy == null)
            Destroy(gameObject);
        else
        {
            transform.position = enemy.transform.position;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && !enemy.isHurt && !enemy.isDead)
        {
            enemy.state = EnemyStates.CHASE;
            enemy.StartChasing();
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
