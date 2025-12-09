using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionFollow : MonoBehaviour
{
    private Transform leader;
    private NavMeshAgent agent;
    public float FollowDistance = 2.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetLeader(GameObject leaderObj)
    {
        if (leaderObj == null) { leader = null; return; }
        leader = leaderObj.transform;
    }

    private void Update()
    {
        if (leader == null || agent == null) return;
        float dst = Vector3.Distance(transform.position, leader.position);
        if (dst > FollowDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(leader.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
