using UnityEngine;

public class StarScript3D : MonoBehaviour
{
    public float size = 0.86f; // sun radius
    public float mass = 1.1f; // suns
    public float temperature = 1; // relative to sun temp
    public float luminosity = 1.5f; // relatuve to sun luminosity
    public float age = 1.1f; // relative to sun age


    public float currentSpeed;
    public bool movingTowardsAnotherStar;
    public float maxDistanceFromAnotherStar;
    public Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        currentSpeed = rb.linearVelocity.magnitude;
    }
}
