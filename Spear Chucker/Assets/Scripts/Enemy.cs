using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public int Health;

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Shader Stuff")]
    public TargetLock targetLock;
    MeshRenderer meshRenderer;
    public Material lockOnMat;
    public Material defaultMat;

    public void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        //In update to constantly check for the player
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        //stuff for changing material when targeted
        if (targetLock.isTargeting && targetLock.currentTarget == transform)
        {
            meshRenderer.material = lockOnMat;
        }
        else
        {
            meshRenderer.material = defaultMat;
        }
    }

    public void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack logic here
            Debug.Log("Enemy Attacked");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spear"))
        {
            Debug.Log("Player hit by enemy");
            TakeDamage(10); // Enemy takes damage when colliding with player
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0) Invoke(nameof(Death), 0.5f);
    }

    private void Death()
    {
        ObjectPool.Instance.ReturnToPool(gameObject);
        DropMeat();
    }

    private void DropMeat()
    {
        GameObject meat = ObjectPool.Instance.SpawnFromPool("Meat", transform.position, Quaternion.identity);
        Rigidbody rb = meat.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
        }
    }
}
