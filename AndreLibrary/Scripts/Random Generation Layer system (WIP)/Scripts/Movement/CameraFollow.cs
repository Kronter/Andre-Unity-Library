using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraPositionOffSet;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, target.position + cameraPositionOffSet, ref velocity, smoothTime);
    }
}
