using UnityEngine;

public class TestSpeedy : MonoBehaviour
{
    public float Speed = 0;
    public float Acceleration = 0;
    public float Weight = 0;
    public float DistanceTraveled = 0;
    public bool Run = false;
    public LayerMask collisionMask;
    public Vector3 BeginingPos;
    private Vector3 FinalPos;
    [SerializeField] private Rigidbody rb;
    private float A = 0.25f;

    Vector3 previousPos;

    private void Start()
    {
        previousPos = transform.position;
    }

    public void resetA()
    {
        A = 0.25f;
        times = 0;
    }

    int times = 0;
    void FixedUpdate()
    {
        if (!Run)
        {
            FinalPos = transform.position;
            DistanceTraveled = Vector3.Distance(BeginingPos, FinalPos);
            return;
        }
        rb.mass = Weight;
        previousPos = transform.position;
    
        if (A >= Speed)
            A = Speed;

        float v = (A / Weight) * Time.fixedDeltaTime;

        transform.Translate(Vector3.right * v);
        if (Andre.Utils.Collision.BulletColision(previousPos, transform.position, collisionMask, false))
        {
            transform.position = previousPos;
            times++;
        }

        if (times > 10)
            Run = false;
        FinalPos = transform.position;
        DistanceTraveled = Vector3.Distance(BeginingPos, FinalPos);

        A += v * Acceleration;
    }
}
