using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// LAB - Commands a NavMeshAgent to walk toward the player's position with agent.SetDestination().
/// Put this on the capsule (which also has a NavMeshAgent). The agent finds its own way around
/// walls and up ramps, because it only walks on the baked NavMesh.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshChase : MonoBehaviour
{
    [Tooltip("The player character. Drag it in from the Hierarchy.")]
    [SerializeField] private Transform player;

    [Tooltip("The agent only plans a new route when the player has moved this far (metres).")]
    [SerializeField] private float repathDistance = 0.25f;

    [Tooltip("How far from the player's feet we search for the nearest spot on the NavMesh (metres).")]
    [SerializeField] private float snapDistance = 2f;

    private NavMeshAgent agent;
    private Vector3 lastDestination = Vector3.positiveInfinity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(name + ": the agent is not on the NavMesh. Bake the NavMesh and place the capsule on it.");
            return;
        }

        // The player might be standing a little off the NavMesh (on a ledge edge, for example),
        // so aim for the nearest valid point instead.
        if (!NavMesh.SamplePosition(player.position, out NavMeshHit hit, snapDistance, NavMesh.AllAreas))
            return;

        // Don't call SetDestination every frame: restarting the route calculation each time
        // can stop the agent from ever finishing a long route.
        bool playerMoved = (hit.position - lastDestination).sqrMagnitude > repathDistance * repathDistance;
        bool needsRoute = !agent.hasPath && !agent.pathPending;

        if (playerMoved || needsRoute)
        {
            agent.SetDestination(hit.position);   // the one call the lab asks for
            lastDestination = hit.position;
        }
    }
}
