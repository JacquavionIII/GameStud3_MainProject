using UnityEngine;
using UnityEngine.VFX;

public class HumanEnemy : Enemy
{
    public float deathTimer = 5f;
    public VisualEffect hitVFX;
    public override void Start()
    {
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (smr.gameObject.name == "Medieval_Body") //We're looking for the specific mesh because this thing keeps on screwing with me in the heal function (fuck I hate shaders)
            {
                meshRenderer = smr;
                break;
            }
        }
    }

    public override void Patroling()
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isRunning", false);
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    protected override void ChasePlayer()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isWalking", false);
        agent.SetDestination(player.position);

        // Ensure the enemy faces the player while chasing
        transform.LookAt(player);
    }

    protected override void AttackPlayer()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", true);
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

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spear"))
        {
            Debug.Log("Enemy Hit by Spear");
            TakeDamage(20); // Enemy takes damage when colliding with player
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy");
            TakeDamage(10); // Enemy takes damage when colliding with player
        }
    }

    private void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0) Invoke(nameof(Death), 0.5f);
    }

    protected override void Death()
    {
        DropMats();
        //I was planning to add a deathVFX to make the enemy dissolve but the problem is that the current enemy i have needs textures which i dont have so making a shader graph will kill me rn
        ComeBackAfterDeath(); //notify subscribers that this enemy has died
        ObjectPool.Instance.ReturnToPool(gameObject); //returns the object to pool once it dies
    }

    protected override void DropMats()
    {
        GameObject wood = ObjectPool.Instance.SpawnFromPool("Wood", transform.position, Quaternion.identity);
        GameObject stone = ObjectPool.Instance.SpawnFromPool("Stone", transform.position, Quaternion.identity);
        Rigidbody rb = wood.GetComponent<Rigidbody>();
        Rigidbody rb2 = stone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
        }
        if (rb2 != null)
        {
            rb2.linearVelocity = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
        }
    }
}
