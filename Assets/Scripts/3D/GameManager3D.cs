using System.Linq;
using UnityEngine;

public class GameManager3D : MonoBehaviour
{
    public static StarScript3D[] stars;
    public float timescale = 1f;
    public float G = 9.81f;
    public float distancing_distanceToApplyDecay = 200f;
    public float distancing_speedDecay = 0.96f;

    public float orbitRadius = 2f;          // Distance from center for initial positions
    public float orbitalSpeedMultiplier = 1f; // Scale for initial velocities

    public void Start()
    {
        // Find all active StarScript3D components
        GameManager3D.stars = GameObject.FindObjectsByType<StarScript3D>(FindObjectsSortMode.None);
        stars = stars.Where(x => x.isActiveAndEnabled).ToArray();
        Debug.Log(stars.Length);

        float rotation_direction = Random.value < 0.5f ? 1f : -1f;

        // Place stars in a spherical distribution instead of a 2D circle
        for (int i = 0; i < stars.Length; i++)
        {
            // Generate random spherical coordinates
            float theta = Random.Range(0f, 2f * Mathf.PI); // Azimuthal angle
            float phi = Random.Range(0f, Mathf.PI);       // Polar angle
            Vector3 position = new Vector3(
                orbitRadius * Mathf.Sin(phi) * Mathf.Cos(theta),
                orbitRadius * Mathf.Sin(phi) * Mathf.Sin(theta),
                orbitRadius * Mathf.Cos(phi)
            );
            stars[i].rb.position = position;
        }

        // Calculate center of mass
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        for (int i = 0; i < stars.Length; i++)
        {
            centerOfMass += stars[i].rb.position * stars[i].mass;
            totalMass += stars[i].mass;
        }
        centerOfMass /= totalMass;

        // Assign initial velocities in 3D
        for (int i = 0; i < stars.Length; i++)
        {
            // Vector from star to center of mass
            Vector3 toCenter = stars[i].rb.position - centerOfMass;
            float distanceToCenter = toCenter.magnitude;

            // Define the orbital plane's normal (randomized for 3D variation)
            Vector3 randomNormal = Random.onUnitSphere; // Random direction in 3D
            Vector3 tangent = Vector3.Cross(toCenter, randomNormal).normalized;

            // Handle edge case where cross product might be zero
            if (tangent.sqrMagnitude < 0.001f)
            {
                tangent = Vector3.Cross(toCenter, Vector3.up).normalized;
                if (tangent.sqrMagnitude < 0.001f)
                {
                    tangent = Vector3.Cross(toCenter, Vector3.right).normalized;
                }
            }

            // Compute orbital speed
            float effectiveMass = totalMass - stars[i].mass;
            float orbitalSpeed = Mathf.Sqrt(G * effectiveMass / distanceToCenter) * orbitalSpeedMultiplier;

            // Assign velocity along the tangent direction
            stars[i].rb.linearVelocity = tangent * orbitalSpeed * rotation_direction;
        }
    }

    private void OnDrawGizmos()
    {
        if (stars == null || stars.Length == 0)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            var dir = stars[i].rb.linearVelocity;
            var color = stars[i].movingTowardsAnotherStar ? Color.green : Color.red;
            Gizmos.color = color;
            Gizmos.DrawLine(stars[i].rb.position, stars[i].rb.position + dir);

            for (int j = 0; j < stars.Length; j++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(stars[i].rb.position, stars[j].rb.position);
            }
        }
    }

    private void FixedUpdate()
    {
        Time.timeScale = timescale;

        for (int i = 0; i < stars.Length; i++)
        {
            // Reset tracking variables (uncomment if still needed in StarScript3D)
            // stars[i].movingTowardsAnotherStar = false;
            // stars[i].maxDistanceFromAnotherStar = -1f;

            for (int j = 0; j < stars.Length; j++)
            {
                if (i != j)
                {
                    // Use Vector3 for 3D direction
                    Vector3 direction = stars[j].rb.position - stars[i].rb.position;

                    // Calculate force magnitude (1/r instead of 1/r� for your original scaling)
                    float forceMagnitude = G * stars[i].mass * stars[j].mass / direction.magnitude;

                    // Apply force to the Rigidbody (in 3D)
                    Vector3 force = direction.normalized * forceMagnitude;
                    stars[i].rb.AddForce(force);

                    // Optional: Uncomment and adapt for 3D if you want to track movement direction
                    // if (Vector3.Dot(stars[i].rb.velocity, direction) < 0)
                    // {
                    //     stars[i].movingTowardsAnotherStar = true;
                    // }
                    //
                    // stars[i].maxDistanceFromAnotherStar = Mathf.Max(stars[i].maxDistanceFromAnotherStar, Vector3.Distance(stars[i].rb.position, stars[j].rb.position));
                }
            }

            // Decay velocity if far away (uncomment and adapt if needed)
            // if (!stars[i].movingTowardsAnotherStar && stars[i].maxDistanceFromAnotherStar > distancing_distanceToApplyDecay)
            // {
            //     stars[i].rb.velocity *= distancing_speedDecay;
            // }
        }
    }
}