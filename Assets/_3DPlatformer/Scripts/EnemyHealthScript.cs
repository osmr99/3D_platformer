using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public enum EnemyStates
{
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyHealthScript : MonoBehaviour
{
    Animator anim;

    [Header("Set up Materials")]
    [SerializeField] private SkinnedMeshRenderer _skin;
    public Material _baseMat;
    public Material _hurtMat;

    [Header("Set up Effects")]
    public GameObject impactEffect;
    public GameObject deathSmokeEffect;
    public Transform impactSpawnPoint;

    [Header("Set up Coin Drop System")]
    public Rigidbody coinObject;
    public int coinCount = 4;
    public float spawnRadius = 0.75f;
    public float spawnHeight = 0.5f;

    // Coin Scattering and Force
    public bool applyForce = true;
    public float forwardForce = 3f;
    public float upwardForce = 1f;

    public int enemyHealth;
    [SerializeField] Rigidbody rb;
    public float knockBackForce = 4;

    [SerializeField] public bool isHurt;
    [SerializeField] public bool isDead;

    PlayerControllerScript playerControllerScript;

    // Enemy AI
    [Header("Enemy AI")]
    public NavMeshAgent agent;
    public EnemyStates state = EnemyStates.PATROL;
    public Transform[] patrollingPoints;
    public bool canPatrol = true;   
    public float delay;
    int randomNum;
    int previousNum;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _skin.material = _baseMat;
        playerControllerScript = FindFirstObjectByType<PlayerControllerScript>();
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyStates.PATROL:
                Patrolling();
                break;
            case EnemyStates.CHASE:
                break;
            //case EnemyStates.ATTACK:
                //break;
        }
    }

    void Patrolling()
    {
        agent.speed = 1.5f;
        if(!agent.hasPath && canPatrol)
        {
            canPatrol = false;
            while (randomNum == previousNum)
                randomNum = Random.Range(0, patrollingPoints.Length - 1);
            previousNum = randomNum;
            agent.SetDestination(patrollingPoints[randomNum].position);
            anim.SetBool("Patrolling", true);

        }
        else if (agent.remainingDistance < 0.1f && anim.GetBool("Patrolling"))
        {
            anim.SetBool("Patrolling", false);
            StopAllCoroutines();
            StartCoroutine(PatrolDelay());
        }
    }

    IEnumerator PatrolDelay()
    {
        yield return new WaitForSeconds(delay);
        canPatrol = true;
    }

    public void StartChasing()
    {
        StopAllCoroutines();
        canPatrol = false;
        anim.SetBool("Patrolling", false);
        agent.speed = 3;
        anim.SetBool("Found", true);
        agent.SetDestination(playerControllerScript.transform.position);
    }

    public void EndChasing()
    {
        agent.speed = 0;
        anim.SetBool("Found", false);
        agent.SetDestination(transform.position);
        StartCoroutine(ChaseDelay());
    }

    IEnumerator ChaseDelay()
    {
        yield return new WaitForSeconds(delay);
        canPatrol = true;
        state = EnemyStates.PATROL;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HitSphere" && !isHurt && !isDead)
        {
            isHurt = true;
            Transform _player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 lookPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookPosition);
            StartCoroutine(TakeDamage());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!playerControllerScript.isHurt && !isDead && !isHurt)
            {
                playerControllerScript.UpdateHP(1);
            }
        }
    }

    IEnumerator TakeDamage()
    {
        enemyHealth--;
        anim.SetTrigger("Hurt");
        Instantiate(impactEffect, impactSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.025f);
        HitStopManager.Instance.DoHitStop(0.15f, 0);
        rb.AddForce(transform.forward * -knockBackForce, ForceMode.Impulse);
        _skin.material = _hurtMat;

        if(enemyHealth > 0)
        {
            yield return new WaitForSeconds(0.1f);
            _skin.material = _baseMat;
            isHurt = false;
        }
        else
        {
            StartCoroutine(EnemyDeath());
        }

    }

    IEnumerator EnemyDeath()
    {
        isDead = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(1.5f);
        Instantiate(deathSmokeEffect, impactSpawnPoint.position, Quaternion.identity);
        DropCoins();
        Destroy(gameObject);
    }

    void DropCoins()
    {
        if (coinObject == null || coinCount <= 0)
            return;
        float angleStep = 360 / coinCount;
        float starAngle = Random.Range(0, 360);

        for(int i = 0; i < coinCount; i++)
        {
            float _angle = (starAngle + angleStep * i)
                * Mathf.Deg2Rad;
            Vector3 _direction = new Vector3(Mathf.Cos(_angle), 
                0f, Mathf.Sin(_angle));
            Vector3 spawnPosition = transform.position + _direction
                * spawnRadius + Vector3.up * spawnHeight;

            Rigidbody _coin = Instantiate(coinObject, spawnPosition, Quaternion.identity);

            if(applyForce)
            {
                Vector3 _force = (_direction + Vector3.up * upwardForce)
                    .normalized * forwardForce;
                _coin.AddForce(_force, ForceMode.Impulse);
            }
        }
    }
}
