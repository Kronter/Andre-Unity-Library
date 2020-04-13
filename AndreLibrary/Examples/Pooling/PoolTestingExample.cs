using UnityEngine;

public class PoolTestingExample : MonoBehaviour
{
    [SerializeField]
    private float refireRate = 2f;

    [SerializeField]
    private float beginDelay = 0.5f;

    [SerializeField]
    private int initialPoolSize = 4;

    private float fireTimer = 0;
    private float delayTimer = 0;

    private void Awake()
    {
        ShotPool.Instance.AddObjects(initialPoolSize);
    }

    private void Update()
    {

        if (delayTimer < beginDelay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        fireTimer += Time.deltaTime;
        if(fireTimer >= refireRate)
        {
            fireTimer = 0;
            Fire();
        }
    }

    private void Fire()
    {
        var shot = ShotPool.Instance.Get();
        shot.transform.rotation = transform.rotation;
        shot.transform.position = transform.position;
        shot.gameObject.SetActive(true);
    }
}