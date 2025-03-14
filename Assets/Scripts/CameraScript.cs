using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DeepUnity
{
    public class CameraScript : MonoBehaviour
    {
        private Vector3 targetToFollow;
        private float targetOrtographicSize;
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
            float stars_aggregated_size = GameManager.stars.Sum(x => x.size);
            targetToFollow = new Vector3(
                GameManager.stars.Sum(x => x.rb.position.x * (x.size / stars_aggregated_size)), 
                GameManager.stars.Sum(x => x.rb.position.y * (x.size / stars_aggregated_size)), 
                transform.position.z);

            // Compute largest distance between two stars
            // a scale of 50 matches a distance of about 200 units
            float maxDistance = -1f;
            for (int i = 0; i < GameManager.stars.Length; i++)
            {
                for (int j = 0; j < GameManager.stars.Length; j++)
                {
                    float dist = Vector3.Distance(GameManager.stars[i].rb.position, GameManager.stars[j].rb.position);
                    maxDistance = Mathf.Max(maxDistance, dist);
                }
            }

            targetOrtographicSize = Mathf.Max(maxDistance / 2f, 50f); // At least 50 units of scale
            

        
        }
        private void LateUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetToFollow + offset, ref speed, smoothness);
            cam.orthographicSize = 0.95f * cam.orthographicSize + 0.05f * targetOrtographicSize;
        }
    }
}

