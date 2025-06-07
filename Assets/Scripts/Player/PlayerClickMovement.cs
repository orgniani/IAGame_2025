using UnityEngine;
using Nodes;

namespace Player
{
    [RequireComponent(typeof(PathNodeAgent))]
    public class PlayerClickMovement : MonoBehaviour
    {
        private Camera mainCamera;
        private PathNodeAgent agent;


        void Awake ()
        {
            agent = GetComponent<PathNodeAgent>();
            mainCamera = Camera.main;
        }

        void Update ()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, maxDistance: float.MaxValue))
                    agent.Destination = raycastHit.point;
            }
        }
    }
}