using System.Linq;
using UnityEngine;

public class GameManager2D : MonoBehaviour
{
    
    public static StarScript2D[] stars;
    public float timescale = 1f;
    public float G = 9.81f;
    public float distancing_distanceToApplyDecay = 200f;
    public float distancing_speedDecay = 0.96f;
    
    public Vector2[] starsInitialVeolicity = new Vector2[3];


    public float orbitRadius = 2f;          // Distance from center for initial positions
    public float orbitalSpeedMultiplier = 1f; // Scale for initial velocities
    public void Start()
    {
        
        GameManager2D.stars = GameObject.FindObjectsByType<StarScript2D>(FindObjectsSortMode.None);
        stars = stars.Where(x => x.isActiveAndEnabled).ToArray();
        Debug.Log(stars.Length);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].GetComponent<TrailRenderer>().enabled = false;
        }


        float rotation_direction = Random.value < 0.5 ? 1f : -1f;
       

        Vector2 center = Vector2.zero;
        float angleStep = 360f / stars.Length; 
        
        for (int i = 0; i < stars.Length; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad; 
            Vector2 position = new Vector2(
                orbitRadius * Mathf.Cos(angle),
                orbitRadius * Mathf.Sin(angle)
            );
            stars[i].rb.position = position;
        }
        
        
        Vector2 centerOfMass = Vector2.zero;
        float totalMass = 0f;
        for (int i = 0; i < stars.Length; i++)
        {
            centerOfMass += stars[i].rb.position * stars[i].mass;
            totalMass += stars[i].mass;
        }
        centerOfMass /= totalMass;
        
        
        for (int i = 0; i < stars.Length; i++)
        {
            Vector2 toCenter = stars[i].rb.position - centerOfMass;
            Vector2 tangent = new Vector2(-toCenter.y, toCenter.x).normalized;
            float distanceToCenter = toCenter.magnitude;
        
            float effectiveMass = totalMass - stars[i].mass;
            float orbitalSpeed = Mathf.Sqrt(G * effectiveMass / distanceToCenter) * orbitalSpeedMultiplier;
            float random_extra_boost = 1f + Random.value / 50f;
            stars[i].rb.linearVelocity = tangent * orbitalSpeed * rotation_direction * (random_extra_boost);
        }
            
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].GetComponent<TrailRenderer>().enabled = true;
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
            Gizmos.DrawLine(stars[i].rb.position, stars[i].rb.position +  dir);


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
            // stars[i].movingTowardsAnotherStar = false;
            // stars[i].maxDistanceFromAnotherStar = -1f;

            for (int j = 0; j < stars.Length; j++)
            {           
                if(i != j)
                {
                    Vector2 direction = stars[j].rb.position - stars[i].rb.position;

                    // Calculate force magnitude
                    float forceMagnitude = G * stars[i].mass * stars[j].mass / (direction.magnitude);

                    // Apply force to the Rigidbody2D
                    Vector2 force = direction.normalized * forceMagnitude;
                    stars[i].rb.AddForce(force);

                    // We need to actually decay the speed when the star is moving apart from one another
                    // Check if the direction of the star is towards the other stars
                    // if (Vector2.Dot(stars[i].rb.velocity, direction) < 0)
                    // {
                    //     stars[i].movingTowardsAnotherStar = true;
                    // }
                    // 
                    // stars[i].maxDistanceFromAnotherStar = Mathf.Max(stars[i].maxDistanceFromAnotherStar, Vector2.Distance(stars[i].rb.position, stars[j].rb.position));
                }

            }


            // Decay velocity if not moving toward any star AND IS CLEAR that is far far away from the others (to do not affect their actual movements)
            // if (!stars[i].movingTowardsAnotherStar && stars[i].maxDistanceFromAnotherStar > distancing_distanceToApplyDecay)
            // {
            //     stars[i].rb.velocity *= distancing_speedDecay;
            // }
        }
    }
}
