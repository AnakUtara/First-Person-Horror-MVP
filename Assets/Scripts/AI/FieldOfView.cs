using System.Collections;
using UnityEngine;

namespace AI
{
    public class FieldOfView : MonoBehaviour
    {
        [Header("Vision Settings")]
        public Transform visionTarget;
        public float viewRadius;
        [Range(0, 360)]
        public float viewAngle;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask targetLayer;
        
        private float _distanceToTarget;
        private Vector3 _directionToTarget;
        public bool isPlayerVisible;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(FieldOfViewRoutine());
        }

        // Update is called once per frame
        // void Update()
        // {
        //     
        // }
        
        private void FieldOfViewCheck()
        {
            Collider[] visionRange = Physics.OverlapSphere(transform.position, viewRadius, targetLayer.value);

            if (visionRange.Length != 0)
            {
                Transform collidedTarget = visionRange[0].transform;
                _directionToTarget = (collidedTarget.position - transform.position).normalized;
                float lookAngle = Vector3.Angle(transform.forward, _directionToTarget);
                float visionCone = viewAngle / 2;
                if (lookAngle < visionCone)
                {
                    _distanceToTarget = Vector3.Distance(transform.position, collidedTarget.position);
                    if (!Physics.Raycast(transform.position, _directionToTarget, _distanceToTarget,
                            obstacleLayer.value))
                    {
                        isPlayerVisible = true;
                    }
                    else
                    {
                        isPlayerVisible = false;
                    }
                }
                else
                {
                    isPlayerVisible = false;
                }
            } else if (isPlayerVisible)
            {
                isPlayerVisible = false;
            }
        }
        
        private IEnumerator FieldOfViewRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                FieldOfViewCheck();
            }
        }
    }
}
