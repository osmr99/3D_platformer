using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField] private bool isHurt;
    [SerializeField] private bool isDead;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _skin.material = _baseMat;
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
