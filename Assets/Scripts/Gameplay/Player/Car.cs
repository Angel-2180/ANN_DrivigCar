using UnityEngine;
using System.Collections;
using PathCreation;


public class CarInput
{
    public float Thrust = 0;
    public float Steering = 0;
}

public class Car : MonoBehaviour
{
    [Header("Movements")]
    public float ThrustSpeed = 1.0f;
    public float ThrustMaxSpeed = 20.0f;
    public float BreaksForce = 20.0f;
    public float SteeringSpeed;
    public float SteeringMaxSpeed;
    public float DriftFactor = 0.95f;

    [Header("Sensors")]
    public LayerMask WallLayerMask;
    public LayerMask FloorLayerMask;

    int id = 0;
    public int ID { get { return id; } }

    private Rigidbody rb;

    private float rotationAngle = 0.0f;

    [HideInInspector] public float DistanceTraveled = 0.0f;
    private PathCreator path;

    private bool stopped = false;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = SteeringMaxSpeed;

        path = FindObjectOfType<PathCreator>();

        rotationAngle = transform.rotation.eulerAngles.y;

        stopped = false;
    }

    public void ComputeDistanceTraveled()
    {
        DistanceTraveled = path.path.GetClosestTimeOnPath(transform.position);
    }

    public void Move(CarInput carInput)
    {
        if (stopped || rb == null)
            return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 20.0f, FloorLayerMask))
        {
            if (hitInfo.collider.CompareTag("Floor"))
            {
                rb.velocity *= 0.3f;
                return;
            }
        }

        float thrust = carInput.Thrust;

        if (thrust > 0.05f)
            rb.velocity += rb.transform.forward * thrust * ThrustSpeed * Time.fixedDeltaTime;
        else if (thrust < -0.05f)
            rb.velocity += rb.velocity.normalized * Time.fixedDeltaTime * BreaksForce * thrust;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, ThrustMaxSpeed);

        rotationAngle += carInput.Steering * SteeringSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(0, rotationAngle, 0));

        Vector3 forwardVelocity = rb.transform.forward * Vector3.Dot(rb.velocity, rb.transform.forward);
        Vector3 rightVelocity = rb.transform.right * Vector3.Dot(rb.velocity, rb.transform.right);

        rb.velocity = forwardVelocity + rightVelocity * DriftFactor;
    }
}
