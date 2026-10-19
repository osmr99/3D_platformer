using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] EnemyHealthScript enemy;
    [SerializeField] GameObject chaseCollider;

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
            chaseCollider.SetActive(false);
            enemy.state = EnemyStates.ATTACK;
            enemy.Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !enemy.isHurt && !enemy.isDead)
        {
            enemy.StopAllCoroutines();
            chaseCollider.SetActive(true);
            enemy.StartChasing();
            enemy.state = EnemyStates.CHASE;
            enemy.anim.SetBool("InRange", false);
        }
    }
}
