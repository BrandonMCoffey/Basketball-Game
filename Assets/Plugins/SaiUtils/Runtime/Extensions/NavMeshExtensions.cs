using UnityEngine.AI;

namespace SaiUtils.Extensions
{
    public static class NavMeshExtensions
    {
        public static bool HasStoppedMoving(this NavMeshAgent agent)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // Done
                        return true;
                    }
                }
            }

            return false;
        }
    }
}