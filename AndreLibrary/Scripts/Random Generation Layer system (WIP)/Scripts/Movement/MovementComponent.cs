using UnityEngine;

public abstract class MovementComponent : MonoBehaviour
{
    protected Vector3 m_destination;
    public Vector3 destination { get { return m_destination; } set { m_destination = value; } }
    protected bool m_walking = false;
    public bool walking { get{ return m_walking; } }
    protected bool m_canMove = true;
    public bool canMove { get { return m_canMove; } set {m_canMove = value; } }
    //[SerializeField] protected float m_stopDistance = 1.0f;
    //public float stopDIstance { set { m_stopDistance = value; } }
    public float m_stopDistance = 1.0f;
    public float speed = 1.0f;
}
