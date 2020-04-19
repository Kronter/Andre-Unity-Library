using UnityEngine;

public class MoveToClickPoint : MonoBehaviour
{
    [SerializeField] private MovementComponent movementComponent;
    private Vector3 m_pos;


    void Start()
    {
        if (!movementComponent)
            Debug.LogError($"You have forgotten to set movementComponent on {transform.name}");
    }

    void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                movementComponent.destination = hit.point;
                m_pos = hit.point;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(m_pos, 0.5f);
    }
}
