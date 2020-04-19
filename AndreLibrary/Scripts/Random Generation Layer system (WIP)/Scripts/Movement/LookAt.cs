using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private Transform lookTarget;

    // Start is called before the first frame update
    void Start()
    {
         if (!movementComponent)
            Debug.LogError($"You have forgotten to set movementComponent on {transform.name}");

        if (!lookTarget)
            Debug.LogError($"You have forgotten to set lookTarget on {transform.name}");
    }

    // Update is called once per frame
    void Update()
    {
        if(!movementComponent.walking)
            transform.LookAt(new Vector3(lookTarget.position.x, transform.position.y,lookTarget.position.z));
    }
}
