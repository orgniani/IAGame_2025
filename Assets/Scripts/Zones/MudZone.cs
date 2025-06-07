using UnityEngine;
using Nodes;

namespace Zones
{
    public class MudZone : MonoBehaviour
    {
        [SerializeField] private float speedReductionPerc = 0.5f;


        void OnTriggerEnter (Collider other)
        {
            PathNodeAgent agent = other.gameObject.GetComponentInParent<PathNodeAgent>();

            if (agent)
                agent.MovementSpeed *= speedReductionPerc;
        }
        
        void OnTriggerExit (Collider other)
        {
            PathNodeAgent agent = other.gameObject.GetComponentInParent<PathNodeAgent>();

            if (agent)
                agent.MovementSpeed /= speedReductionPerc;
        }
    }
}