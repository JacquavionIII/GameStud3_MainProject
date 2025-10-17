using UnityEngine;

public class HumanEnemy : Enemy
{
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
}
