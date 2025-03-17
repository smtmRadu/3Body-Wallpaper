using System.Linq;
using UnityEngine;

namespace DeepUnity
{
    public class CameraScript3D : MonoBehaviour
    {
        private Vector3 targetToFollow;
        public float smoothness = 0.3f;
        private Vector3 offset;
        private Vector3 speed;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            // we perform a weighted centroid, based on the size of the star
            float stars_aggregated_size = GameManager3D.stars.Sum(x => x.size);
            targetToFollow = new Vector3(
                GameManager3D.stars.Sum(x => x.rb.position.x * (x.size / stars_aggregated_size)), 
                GameManager3D.stars.Sum(x => x.rb.position.y * (x.size / stars_aggregated_size)),
                GameManager3D.stars.Sum(x => x.rb.position.z * (x.size / stars_aggregated_size)));

            // Compute largest distance between two stars
            // a scale of 50 matches a distance of about 200 units
            float maxDistance = -1f;
            for (int i = 0; i < GameManager3D.stars.Length; i++)
            {
                for (int j = 0; j < GameManager3D.stars.Length; j++)
                {
                    float dist = Vector3.Distance(GameManager3D.stars[i].rb.position, GameManager3D.stars[j].rb.position);
                    maxDistance = Mathf.Max(maxDistance, dist);
                }
            }
        
            
        }
        private void LateUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetToFollow + offset, ref speed, smoothness);
        }
    }
}

